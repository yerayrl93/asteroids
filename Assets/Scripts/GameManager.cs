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
        // 1. UI de nivel
        textoNivel.text = "NIVEL " + nivelActual;
        textoNivel.gameObject.SetActive(true);

        // 2. Activar escudo al jugador al inicio de cada nivel
        Jugador player = FindFirstObjectByType<Jugador>();
        if (player != null)
        {
            player.ActivarEscudoTemporal(3f); // 3 segundos de invulnerabilidad
        }

        yield return new WaitForSeconds(2f);
        textoNivel.gameObject.SetActive(false);

        // 3. Spawn de Asteroides según nivel
        int cantidadASpawnear = asteroidesBase + nivelActual;
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            if (spawner != null) spawner.SpawnIndividual();
        }
    }

    public void CheckNivelCompletado()
    {
        // Pequeño retraso para dejar que los objetos se destruyan/desactiven
        Invoke("ValidarEnemigosRestantes", 0.1f);
    }

    private void ValidarEnemigosRestantes()
    {
        // Contar Asteroides activos (usando Pool)
        GameObject[] asteroides = GameObject.FindGameObjectsWithTag("Asteroide");
        int contadorAsteroides = 0;
        foreach (GameObject ast in asteroides) if (ast.activeInHierarchy) contadorAsteroides++;

        // Contar Naves Enemigas
        GameObject[] naves = GameObject.FindGameObjectsWithTag("EnemigoNave");
        int contadorNaves = naves.Length;

        // Si la pantalla está limpia, siguiente nivel
        if (contadorAsteroides == 0 && contadorNaves == 0)
        {
            StopAllCoroutines();
            nivelActual++;

            // Aumento de dificultad progresivo
            if (multiplicadorVelocidad < 2.0f)
                multiplicadorVelocidad += 0.1f;

            StartCoroutine(IniciarNuevoNivel());
        }
    }

    public void Morir()
    {
        PlayerPrefs.SetInt("PuntajeFinal", puntos);
        SceneManager.LoadScene("Final");
    }
}