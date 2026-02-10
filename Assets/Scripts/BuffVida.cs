using UnityEngine;

public class BuffVida : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Esto imprimirá en la consola el Tag de lo que sea que toque la vida
        Debug.Log("Vida tocada por: " + other.name + " con Tag: " + other.tag);

        // Si tu Tag en Unity es "Jugador", aquí debe decir "Jugador"
        if (other.CompareTag("Jugador") || other.CompareTag("Player"))
        {
            Jugador jug = other.GetComponent<Jugador>();
            if (jug != null)
            {
                jug.RecuperarVida();
                Debug.Log("¡Vida entregada con éxito!");
            }

            gameObject.SetActive(false); // Regresa al Pool
        }
    }
}