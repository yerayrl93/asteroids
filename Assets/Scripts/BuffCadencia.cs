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
            // Cambiamos el nombre para que coincida con el script del Jugador
            other.GetComponent<Jugador>().AplicarBuffCadencia(duracionBuff);

            // Destruimos la batería al recogerla
            Destroy(gameObject);
        }
    }
}