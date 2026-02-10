using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class VidaPool : MonoBehaviour
{
    public static VidaPool Instance;

    [Header("Configuración")]
    [SerializeField] private GameObject vidaPrefab;
    [SerializeField] private int vidasIniciales = 5; // Empezamos con 5

    private List<GameObject> poolVidas = new List<GameObject>();

    private void Awake() => Instance = this;

    void Start()
    {
        // Al empezar, creamos los 5 iconos iniciales
        for (int i = 0; i < vidasIniciales; i++)
        {
            GameObject icono = Instantiate(vidaPrefab, transform);
            poolVidas.Add(icono);
        }
    }

    public void RestarVidaVisual()
    {
        // Apaga el último icono que esté encendido
        for (int i = poolVidas.Count - 1; i >= 0; i--)
        {
            if (poolVidas[i].activeSelf)
            {
                poolVidas[i].SetActive(false);
                break;
            }
        }
    }

    public void SumarVidaVisual()
    {
        // 1. Primero buscamos si hay algún icono APAGADO para reciclarlo
        for (int i = 0; i < poolVidas.Count; i++)
        {
            if (!poolVidas[i].activeSelf)
            {
                poolVidas[i].SetActive(true);
                return; // Ya hemos encendido uno, salimos de la función
            }
        }

        // 2. Si llegamos aquí, es que todos están encendidos y necesitamos uno NUEVO
        // Esto es lo que hará que aparezcan más de 5 iconos
        GameObject nuevoIcono = Instantiate(vidaPrefab, transform);
        poolVidas.Add(nuevoIcono);
    }
}