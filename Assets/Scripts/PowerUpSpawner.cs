using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject vidaPrefab;
    public float tiempoMin = 10f; // Tiempo mínimo para que aparezca
    public float tiempoMax = 20f; // Tiempo máximo
    public float radioSpawn = 8f; // Radio desde el centro

    void Start()
    {
        // Inicia el ciclo de aparición
        Invoke("SpawnVida", Random.Range(tiempoMin, tiempoMax));
    }

    void SpawnVida()
    {
        // Calculamos una posición aleatoria dentro de un círculo
        Vector2 posicionAleatoria = Random.insideUnitCircle * radioSpawn;

        // Creamos la vida
        Instantiate(vidaPrefab, posicionAleatoria, Quaternion.identity);

        // Volvemos a llamar a la función para que sea un ciclo infinito
        Invoke("SpawnVida", Random.Range(tiempoMin, tiempoMax));
    }
}