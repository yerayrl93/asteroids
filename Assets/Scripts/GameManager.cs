using System.Collections;
using TMPro; // Necesitas importar TextMeshPro
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
    [SerializeField] private TextMeshProUGUI textoNivel; // Arrastra tu texto aquí
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
        // 1. Mostrar texto de nivel
        textoNivel.text = "NIVEL " + nivelActual;
        textoNivel.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f); // El texto se queda 2 segundos

        textoNivel.gameObject.SetActive(false);

        // 2. Spawnear asteroides (Dificultad por cantidad)
        int cantidadASpawnear = asteroidesBase + nivelActual;
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            spawner.SpawnIndividual();
        }
    }

    public void CheckNivelCompletado()
    {
        // Buscamos cuántos asteroides hay activos en la jerarquía
        GameObject[] asteroidesActivos = GameObject.FindGameObjectsWithTag("Asteroide");

        // Contamos solo los que realmente están encendidos
        int contadorReal = 0;
        foreach (GameObject ast in asteroidesActivos)
        {
            if (ast.activeInHierarchy) contadorReal++;
        }

        // Usamos solo UN bloque de control con una bandera (bool) para evitar ejecuciones dobles
        // Si quedan 0, pasamos de nivel
        if (contadorReal == 0)
        {
            // Detenemos cualquier intento previo de iniciar nivel para evitar bugs
            StopAllCoroutines();

            nivelActual++;

            // Dificultad equilibrada (Tope de 2.0x)
            if (multiplicadorVelocidad < 2.0f)
            {
                multiplicadorVelocidad += 0.1f; // Sube de 10% en 10%
            }

            StartCoroutine(IniciarNuevoNivel());
        }
    }

    public void Morir()
    {
        // Guardamos los puntos en la memoria del PC/Móvil
        PlayerPrefs.SetInt("PuntajeFinal", puntos);
        // Cargamos la escena final
        SceneManager.LoadScene("Final");
    }
}