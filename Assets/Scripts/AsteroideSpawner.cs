using UnityEngine;

public class AsteroideSpawner : MonoBehaviour
{
    public float tiempoSpawn = 2f;

    [Header("Área de Spawn")]
    public float anchoLimite = 8f; // Mitad del ancho de tu pantalla
    public float altoLimite = 4.5f; // Mitad del alto de tu pantalla


    public void SpawnIndividual()
    {
        GameObject ast = AsteroidePool.Instance.GetAsteroide();

        float xAleatorio = Random.Range(-anchoLimite, anchoLimite);
        float yAleatorio = Random.Range(-altoLimite, altoLimite);
        ast.transform.position = new Vector2(xAleatorio, yAleatorio);

        Asteroide script = ast.GetComponent<Asteroide>();
        script.nivel = 3;

        // Aplicamos la dificultad de velocidad del GameManager
        script.velocidadDificultad = GameManager.Instance.multiplicadorVelocidad;

        ast.SetActive(true);
    }
}