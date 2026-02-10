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

    [Header("Interfaz (UI)")]
    [SerializeField] private TextMeshProUGUI textoNivel;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    private AsteroideSpawner spawner;

    private void Awake() { Instance = this; }

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
    }

    public void CheckNivelCompletado()
    {
        // Aumentamos un poco el tiempo para dar margen a la destrucción
        Invoke("ValidarEnemigosRestantes", 0.2f);
    }

    private void ValidarEnemigosRestantes()
    {
        // 1. Contar asteroides activos
        GameObject[] asteroides = GameObject.FindGameObjectsWithTag("Asteroide");
        int contadorAsteroides = 0;
        foreach (GameObject ast in asteroides) if (ast.activeInHierarchy) contadorAsteroides++;

        // 2. Contar naves enemigas que NO estén muriendo
        GameObject[] naves = GameObject.FindGameObjectsWithTag("EnemigoNave");
        int contadorNaves = 0;
        foreach (GameObject nave in naves)
        {
            // Solo contamos la nave si su collider está activo (si está muriendo, el collider se apaga)
            if (nave.GetComponent<Collider2D>().enabled) contadorNaves++;
        }

        if (contadorAsteroides == 0 && contadorNaves == 0)
        {
            nivelActual++;
            if (multiplicadorVelocidad < 2.0f) multiplicadorVelocidad += 0.1f;
            StartCoroutine(IniciarNuevoNivel());
        }
    }

    public void Morir()
    {
        PlayerPrefs.SetInt("PuntajeFinal", puntos);
        PlayerPrefs.SetInt("NivelFinal", nivelActual); // Guardamos también el nivel para la pantalla final
        SceneManager.LoadScene("Final");
    }
}