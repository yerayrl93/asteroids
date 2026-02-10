using System.Collections.Generic;
using UnityEngine;

public class BuffPool : MonoBehaviour
{
    public static BuffPool Instance;
    public GameObject prefabCadencia;
    public GameObject prefabVida;
    public int cantidadInicial = 5;

    private List<GameObject> poolCadencia = new List<GameObject>();
    private List<GameObject> poolVidas = new List<GameObject>();

    private void Awake() => Instance = this;

    void Start()
    {
        for (int i = 0; i < cantidadInicial; i++)
        {
            CrearNuevoItem(prefabCadencia, poolCadencia);
            CrearNuevoItem(prefabVida, poolVidas);
        }
    }

    private GameObject CrearNuevoItem(GameObject prefab, List<GameObject> lista)
    {
        // El 'transform' hace que el clon sea hijo de este objeto (Jerarquía limpia)
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        lista.Add(obj);
        return obj;
    }

    public GameObject GetBuff(bool esVida = false)
    {
        List<GameObject> lista = esVida ? poolVidas : poolCadencia;
        foreach (GameObject obj in lista)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        return CrearNuevoItem(esVida ? prefabVida : prefabCadencia, lista);
    }
}