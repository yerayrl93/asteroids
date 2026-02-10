using UnityEngine;

public class AsteroideSpawner : MonoBehaviour
{
    public float tiempoSpawn = 2f;

    [Header("Área de Spawn")]
    public float anchoLimite = 8f;
    public float altoLimite = 4.5f;

    [Header("Referencias Power-Ups")]
    public GameObject bateríaPrefab; // <--- ARRASTRA LA BATERÍA AQUÍ EN EL INSPECTOR

    [Header("Nave Enemiga")]
    public GameObject naveEnemigaPrefab;
    [Range(0, 100)] public float probabilidadNave = 15f;
    [Range(0, 100)] public float probabilidadOro = 10f;

    public void SpawnIndividual()
    {
        float suerte = Random.Range(0f, 100f);

        if (suerte < probabilidadNave && naveEnemigaPrefab != null)
        {
            GameObject nave = Instantiate(naveEnemigaPrefab, GenerarPosicion(), Quaternion.identity);

            // También le pasamos la batería a la nave por si acaso
            EnemigoNave scriptNave = nave.GetComponent<EnemigoNave>();
            if (scriptNave != null) scriptNave.prefabBateriaBuff = bateríaPrefab;
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

            // --- ESTA ES LA CLAVE ---
            // Si es oro, le recordamos cuál es el prefab de la batería
            if (seraOro)
            {
                script.prefabBateriaBuff = bateríaPrefab;
            }

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