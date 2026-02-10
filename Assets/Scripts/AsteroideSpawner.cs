using UnityEngine;

public class AsteroideSpawner : MonoBehaviour
{
    public float tiempoSpawn = 2f;

    [Header("Área de Spawn")]
    public float anchoLimite = 8f;
    public float altoLimite = 4.5f;

    [Header("Nave Enemiga")]
    public GameObject naveEnemigaPrefab; // Arrastra aquí el prefab de la nave que dispara
    [Range(0, 100)] public float probabilidadNave = 15f;
    [Range(0, 100)] public float probabilidadOro = 10f;

    public void SpawnIndividual()
    {
        float suerte = Random.Range(0f, 100f);

        // 1. ¿Aparece una nave enemiga?
        if (suerte < probabilidadNave && naveEnemigaPrefab != null)
        {
            Instantiate(naveEnemigaPrefab, GenerarPosicion(), Quaternion.identity);
        }
        else // 2. Si no, aparece un asteroide (Normal u Oro)
        {
            GameObject ast = AsteroidePool.Instance.GetAsteroide();
            ast.transform.position = GenerarPosicion();

            Asteroide script = ast.GetComponent<Asteroide>();
            script.nivel = 3;

            // Decidir si este asteroide del pool será de oro esta vez
            script.esOro = (Random.Range(0f, 100f) < probabilidadOro);

            script.velocidadDificultad = GameManager.Instance.multiplicadorVelocidad;
            ast.SetActive(true);
        }
    }

    private Vector2 GenerarPosicion()
    {
        float x = Random.Range(-anchoLimite, anchoLimite);
        float y = Random.Range(-altoLimite, altoLimite);
        return new Vector2(x, y);
    }
}