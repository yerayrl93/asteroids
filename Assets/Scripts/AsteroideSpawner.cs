using UnityEngine;

public class AsteroideSpawner : MonoBehaviour
{
    public float tiempoSpawn = 2f;

    [Header("Área de Spawn")]
    public float anchoLimite = 8f;
    public float altoLimite = 4.5f;

    [Header("Nave Enemiga")]
    public GameObject naveEnemigaPrefab;
    [Range(0, 100)] public float probabilidadNave = 15f;
    [Range(0, 100)] public float probabilidadOro = 10f;

    public void SpawnIndividual()
    {
        float suerte = Random.Range(0f, 100f);

        if (suerte < probabilidadNave && naveEnemigaPrefab != null)
        {
            // Creamos la nave (Las naves suelen ser pocas, el Instantiate aquí está bien, 
            // aunque podrías hacerle un Pool en el futuro si quisieras el 10 plus).
            Instantiate(naveEnemigaPrefab, GenerarPosicion(), Quaternion.identity);

            // NOTA: Ya no asignamos "scriptNave.prefabBateriaBuff" porque la nave 
            // ahora usa internamente el BuffPool.Instance
        }
        else
        {
            GameObject ast = AsteroidePool.Instance.GetAsteroide();
            if (ast == null) return;

            ast.transform.position = GenerarPosicion();
            Asteroide script = ast.GetComponent<Asteroide>();

            script.nivel = 3;

            // Decidimos si es oro
            bool seraOro = (Random.Range(0f, 100f) < probabilidadOro);
            script.esOro = seraOro;

            // --- LIMPIEZA REALIZADA ---
            // Ya NO intentamos acceder a script.prefabBateriaBuff porque el script Asteroide
            // ahora gestiona sus propios drops a través del BuffPool.Instance.GetBuff()

            script.velocidadDificultad = (GameManager.Instance != null) ? GameManager.Instance.multiplicadorVelocidad : 1f;
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