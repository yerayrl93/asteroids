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

        yield return new WaitForSeconds(2f);

        textoNivel.gameObject.SetActive(false);

        // Spawn de Asteroides
        int cantidadASpawnear = asteroidesBase + nivelActual;
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            spawner.SpawnIndividual();
        }

        // Opcional: Podrías añadir aquí lógica para spawnear naves según el nivel
    }

    public void CheckNivelCompletado()
    {
        // Llamamos a la validación con un pequeñísimo retraso
        // Esto es para que el objeto que acaba de morir se destruya del todo
        Invoke("ValidarEnemigosRestantes", 0.1f);
    }

    private void ValidarEnemigosRestantes()
    {
        // 1. Contar Asteroides
        GameObject[] asteroides = GameObject.FindGameObjectsWithTag("Asteroide");
        int contadorAsteroides = 0;
        foreach (GameObject ast in asteroides) if (ast.activeInHierarchy) contadorAsteroides++;

        // 2. Contar Naves Enemigas
        GameObject[] naves = GameObject.FindGameObjectsWithTag("EnemigoNave");
        int contadorNaves = naves.Length; // Las naves se destruyen, no se desactivan

        Debug.Log($"Enemigos restantes: Asteroides({contadorAsteroides}) Naves({contadorNaves})");

        // 3. Si ambos están en cero, pasar nivel
        if (contadorAsteroides == 0 && contadorNaves == 0)
        {
            StopAllCoroutines();
            nivelActual++;

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