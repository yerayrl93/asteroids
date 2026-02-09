using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Ajustes de Bala")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Usamos transform.up porque rotamos el prefab para que su punta sea el eje Y
        rb.linearVelocity = transform.up * velocidad;

        // Se destruye automáticamente para limpiar la jerarquía
        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con un asteroide (o pared si quieres), se destruye
        if (collision.CompareTag("Asteroide"))
        {
            Destroy(gameObject);
        }
    }
}