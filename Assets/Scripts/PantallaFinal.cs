using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PantallaFinal : MonoBehaviour
{
    public TextMeshProUGUI textoPuntaje;
    [SerializeField] private TextMeshProUGUI textoNivel;

    void Start()
    {
        // Leemos la "maleta" que guardamos en el GameManager
        int puntosLogrados = PlayerPrefs.GetInt("PuntajeFinal", 0);
        textoPuntaje.text = "PUNTUACIÓN TOTAL: " + puntosLogrados;

        // Recuperamos el nivel (por defecto 1 si no hay nada)
        int nivel = PlayerPrefs.GetInt("NivelFinal", 1);
        textoNivel.text = "LLEGASTE AL NIVEL: " + nivel.ToString();
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