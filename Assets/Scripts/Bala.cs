using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Ajustes de Bala")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        // Obtenemos la referencia una sola vez para ahorrar recursos
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // IMPORTANTE: Al usar Pool, cada vez que se activa la bala 
        // debemos resetear su velocidad y dirección
        if (rb != null)
        {
            rb.linearVelocity = transform.up * velocidad;
        }

        // En lugar de Destroy, usamos Invoke para "apagarla" tras el tiempo de vida
        Invoke("Desactivar", tiempoVida);
    }

    private void OnDisable()
    {
        // Cancelamos cualquier Invoke pendiente al desactivar para evitar bugs
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con un enemigo, asteroide o bala enemiga
        if (collision.CompareTag("Asteroide") || collision.CompareTag("EnemigoNave") || collision.CompareTag("BalaEnemigo"))
        {
            Desactivar();
        }
    }

    private void Desactivar()
    {
        // El Pool Object consiste en desactivar, NO destruir
        gameObject.SetActive(false);
    }
}