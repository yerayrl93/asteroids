using System.Collections.Generic;
using UnityEngine;

public class BalaPool : MonoBehaviour
{
    public static BalaPool Instance;

    [Header("Configuración")]
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private int tamañoInicial = 20;

    private List<GameObject> poolDeBalas = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Llenamos el pool al empezar el juego
        for (int i = 0; i < tamañoInicial; i++)
        {
            CrearNuevaBala();
        }
    }

    private GameObject CrearNuevaBala()
    {
        GameObject bala = Instantiate(balaPrefab);
        bala.SetActive(false); // Las creamos "apagadas"
        poolDeBalas.Add(bala);
        return bala;
    }

    public GameObject GetBala()
    {
        // Buscamos una bala que no esté en uso
        foreach (GameObject bala in poolDeBalas)
        {
            if (!bala.activeInHierarchy)
            {
                return bala;
            }
        }

        // Si todas están en uso, creamos una nueva para no quedarnos cortos
        return CrearNuevaBala();
    }
}