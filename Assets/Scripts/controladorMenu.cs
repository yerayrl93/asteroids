using UnityEngine;
using UnityEngine.SceneManagement; // Obligatorio para cambiar de escenas

public class ControladorMenu : MonoBehaviour
{
    // 1. Para el botón JUGAR
    public void CargarJuego()
    {
        SceneManager.LoadScene("Asteroids"); // Asegúrate de que se llame así
    }

    // 2. Para el botón ACERCA DE (Cine)
    public void IrAlCine()
    {
        SceneManager.LoadScene("EscenaCine");
    }

    // 3. Para el botón VOLVER (Dentro de la EscenaCine)
    public void IrAlMenu()
    {
        SceneManager.LoadScene("Menu"); // El nombre de tu escena principal
    }

    // 4. Para el botón SALIR
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}