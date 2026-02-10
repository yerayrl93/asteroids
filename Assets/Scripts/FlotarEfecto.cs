using UnityEngine;

public class FlotarEfecto : MonoBehaviour
{
    public float amplitud = 0.5f;
    public float frecuencia = 1f;
    Vector3 posInicial;

    void Start() => posInicial = transform.position;

    void Update()
    {
        // Movimiento sinusoidal suave
        float nuevoY = posInicial.y + Mathf.Sin(Time.time * frecuencia) * amplitud;
        transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
    }
}