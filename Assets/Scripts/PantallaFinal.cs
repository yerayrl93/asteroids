using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PantallaFinal : MonoBehaviour
{
    public TextMeshProUGUI textoPuntaje;

    void Start()
    {
        // Leemos la "maleta" que guardamos en el GameManager
        int puntosLogrados = PlayerPrefs.GetInt("PuntajeFinal", 0);
        textoPuntaje.text = "PUNTUACIÓN TOTAL: " + puntosLogrados;
    }

    public void Reintentar()
    {
        SceneManager.LoadScene("Asteroids");
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }
}