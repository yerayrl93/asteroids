using UnityEngine;

public class balaEnemiga : MonoBehaviour
{
    [SerializeField] private float tiempoDeVida = 4f;

    private void OnEnable()
    {
        // En lugar de Destroy, usamos Invoke para "apagarla" y que vuelva al Pool
        Invoke("Desactivar", tiempoDeVida);
    }

    private void OnDisable()
    {
        // Importante cancelar el Invoke para que no se apague sola cuando ya está en el Pool
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignoramos colisiones con otros enemigos o asteroides
        if (other.CompareTag("Asteroide") || other.CompareTag("EnemigoNave")) return;

        if (other.CompareTag("Player"))
        {
            Jugador scriptJugador = other.GetComponent<Jugador>();
            if (scriptJugador != null) scriptJugador.TomarDaño();
            Desactivar();
        }

        if (other.CompareTag("Limite"))
        {
            Desactivar();
        }
    }

    private void Desactivar()
    {
        gameObject.SetActive(false);
    }
}