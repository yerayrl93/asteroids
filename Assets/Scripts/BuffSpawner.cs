using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject prefabBuff;
    [SerializeField] private float tiempoEntreSpawns = 15f; // Cada 15 segundos

    private float limiteX;
    private float limiteY;

    void Start()
    {
        // Calculamos los límites de la pantalla
        Camera cam = Camera.main;
        limiteY = cam.orthographicSize - 1f; // -1 para que no aparezca justo en el borde
        limiteX = (limiteY * cam.aspect) - 1f;

        // Iniciamos la repetición: (NombreMetodo, tiempoInicio, tiempoRepeticion)
        InvokeRepeating("SpawnBuffAleatorio", tiempoEntreSpawns, tiempoEntreSpawns);
    }

    void SpawnBuffAleatorio()
    {
        if (prefabBuff == null) return;

        // Generamos posición aleatoria dentro de los límites de la cámara
        float posX = Random.Range(-limiteX, limiteX);
        float posY = Random.Range(-limiteY, limiteY);
        Vector3 posicionSpawn = new Vector3(posX, posY, 0f);

        // Instanciamos el buff
        GameObject buff = BuffPool.Instance.GetBuff();
        if (buff != null)
        {
            buff.transform.position = posicionSpawn; // La posición aleatoria
            buff.SetActive(true);
        }
    }
}