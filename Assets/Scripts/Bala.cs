using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Ajustes de Bala")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 2f;

    private Rigidbody2D rb;

    private void Start() // Cambiado a Start para asegurar que el RB esté listo
    {
        rb = GetComponent<Rigidbody2D>();

        // IMPORTANTE: Le damos velocidad hacia "arriba" relativo a la bala
        // transform.up es la dirección hacia donde apunta el frente de la bala
        rb.linearVelocity = transform.up * velocidad;

        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroide"))
        {
            Destroy(gameObject);
        }
    }
}