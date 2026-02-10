using System.Collections.Generic;
using UnityEngine;

public class BalaEnemigaPool : MonoBehaviour
{
    // Singleton único para las balas enemigas
    public static BalaEnemigaPool Instance;

    [Header("Configuración del Pool")]
    [SerializeField] private GameObject prefabBalaEnemiga;
    [SerializeField] private int tamanoInicial = 10;

    private List<GameObject> poolDeBalas = new List<GameObject>();

    private void Awake()
    {
        // Si ya existe una instancia, la destruimos para evitar conflictos
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Llenamos el pool al iniciar
        for (int i = 0; i < tamanoInicial; i++)
        {
            CrearNuevaBala();
        }
    }

    private GameObject CrearNuevaBala()
    {
        GameObject bala = Instantiate(prefabBalaEnemiga);
        bala.SetActive(false);
        // Opcional: Mantener la jerarquía limpia metiéndolas dentro del Pool
        bala.transform.SetParent(this.transform);
        poolDeBalas.Add(bala);
        return bala;
    }

    public GameObject GetBalaEnemiga()
    {
        // Buscamos una bala que esté apagada
        foreach (GameObject bala in poolDeBalas)
        {
            if (!bala.activeInHierarchy)
            {
                return bala;
            }
        }

        // Si no hay ninguna libre, creamos una nueva (Pool dinámico)
        return CrearNuevaBala();
    }
}