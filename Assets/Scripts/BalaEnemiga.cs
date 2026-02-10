using UnityEngine;

public class balaEnemiga : MonoBehaviour
{
    [SerializeField] private float tiempoDeVida = 4f;

    void Start()
    {
        // La bala se destruye sola después de unos segundos si no choca con nada
        // Esto evita que el juego se ralentice por tener miles de balas perdidas
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si la bala toca a un enemigo (asteroide o la nave que la disparó), la ignoramos
        if (other.CompareTag("Asteroide") || other.CompareTag("EnemigoNave")) return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Jugador>().TomarDaño();
            Destroy(gameObject);
        }

        if (other.CompareTag("Limite"))
        {
            Destroy(gameObject);
        }
    }
}