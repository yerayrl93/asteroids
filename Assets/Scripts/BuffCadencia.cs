using UnityEngine;

public class BuffCadencia : MonoBehaviour
{
    [SerializeField] private float duracionBuff = 5f;
    [SerializeField] private float velocidadRotacion = 100f;

    void Update()
    {
        // La batería gira para que se vea que es un ítem especial
        transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el componente Jugador
            Jugador jugador = other.GetComponent<Jugador>();

            if (jugador != null)
            {
                jugador.AplicarBuffCadencia(duracionBuff);
            }

            // --- CAMBIO PARA POOLING ---
            // En lugar de Destroy(gameObject), lo desactivamos
            gameObject.SetActive(false);
        }
    }
}