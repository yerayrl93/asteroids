using System.Collections.Generic;
using UnityEngine;

public class AsteroidePool : MonoBehaviour
{
    public static AsteroidePool Instance;
    [SerializeField] private GameObject asteroidePrefab;
    [SerializeField] private int cantidadInicial = 20;
    private List<GameObject> pool = new List<GameObject>();

    private void Awake() { Instance = this; }

    private void Start()
    {
        for (int i = 0; i < cantidadInicial; i++)
        {
            GameObject obj = Instantiate(asteroidePrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetAsteroide()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        // Si el pool es pequeño, creamos uno nuevo
        GameObject nuevoObj = Instantiate(asteroidePrefab);
        pool.Add(nuevoObj);
        return nuevoObj;
    }
}