using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Ajustes de Bala")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 2f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // La bala sale disparada hacia donde apunta el cañón
        rb.linearVelocity = transform.up * velocidad;

        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Añadimos "EnemigoNave" a la lista de cosas que destruyen la bala
        if (collision.CompareTag("Asteroide") || collision.CompareTag("EnemigoNave"))
        {
            // NOTA: No hace falta destruir la nave aquí, 
            // porque la propia nave ya tiene su script que detecta "Bala" y se destruye.
            Destroy(gameObject);
        }
    }
}