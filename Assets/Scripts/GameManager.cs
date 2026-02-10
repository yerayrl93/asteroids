using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Nivel")]
    public int nivelActual = 1;
    public int asteroidesBase = 2;
    public float multiplicadorVelocidad = 1f;
    private int puntos = 0;
    private bool cambiandoDeNivel = false; // Evita que se disparen varios niveles a la vez

    [Header("Interfaz (UI)")]
    [SerializeField] private TextMeshProUGUI textoNivel;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    private AsteroideSpawner spawner;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        spawner = FindFirstObjectByType<AsteroideSpawner>();
        StartCoroutine(IniciarNuevoNivel());
    }

    public void GanarPuntos(int cantidad)
    {
        puntos += cantidad;
        textoPuntos.text = "SCORE: " + puntos;
    }

    public IEnumerator IniciarNuevoNivel()
    {
        cambiandoDeNivel = true; // Bloqueamos el chequeo

        textoNivel.text = "NIVEL " + nivelActual;
        textoNivel.gameObject.SetActive(true);

        Jugador player = FindFirstObjectByType<Jugador>();
        if (player != null) player.ActivarEscudoTemporal(3f);

        yield return new WaitForSeconds(2f);
        textoNivel.gameObject.SetActive(false);

        int cantidadASpawnear = asteroidesBase + nivelActual;
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            if (spawner != null) spawner.SpawnIndividual();
        }

        // Damos un pequeño margen antes de permitir el chequeo de victoria
        yield return new WaitForSeconds(1f);
        cambiandoDeNivel = false;
    }

    public void CheckNivelCompletado()
    {
        // Si ya estamos cambiando de nivel, no hagas nada
        if (cambiandoDeNivel) return;

        // Usamos una Corrutina en lugar de Invoke para tener más control
        StartCoroutine(ValidarEnemigosCorrutina());
    }

    private IEnumerator ValidarEnemigosCorrutina()
    {
        // Esperamos al final del frame para que los hijos de los asteroides 
        // tengan tiempo de activarse en la jerarquía
        yield return new WaitForEndOfFrame();

        // 1. Contar asteroides activos
        GameObject[] asteroides = GameObject.FindGameObjectsWithTag("Asteroide");
        int contadorAsteroides = 0;
        foreach (GameObject ast in asteroides)
        {
            if (ast.activeInHierarchy) contadorAsteroides++;
        }

        // 2. Contar naves enemigas
        GameObject[] naves = GameObject.FindGameObjectsWithTag("EnemigoNave");
        int contadorNaves = 0;
        foreach (GameObject nave in naves)
        {
            // Solo contamos la nave si tiene el sprite o el collider activo
            if (nave.activeInHierarchy) contadorNaves++;
        }

        // Solo si realmente no queda NADA, avanzamos
        if (contadorAsteroides == 0 && contadorNaves == 0 && !cambiandoDeNivel)
        {
            cambiandoDeNivel = true;
            nivelActual++;

            // Dificultad progresiva más suave
            if (multiplicadorVelocidad < 2.5f) multiplicadorVelocidad += 0.15f;

            StartCoroutine(IniciarNuevoNivel());
        }
    }

    public void Morir()
    {
        PlayerPrefs.SetInt("PuntajeFinal", puntos);
        PlayerPrefs.SetInt("NivelFinal", nivelActual);
        SceneManager.LoadScene("Final");
    }
}