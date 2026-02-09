using UnityEngine;

public class Jugador : MonoBehaviour
{
    [Header("Parametros Nave")]
    [SerializeField] private float aceleracionNave = 10f;
    [SerializeField] private float maximaVelocidad = 10f;
    [SerializeField] private float rotacionVelocidad = 180f;

    [Header("Disparo")]
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 0.3f;

    private Rigidbody2D naveRigidbody;
    private bool estaVivo = true;
    private bool estaAcelerando = false;
    private float direccionRotacion = 0f;
    private float tiempoProximoDisparo;

    private void Start()
    {
        naveRigidbody = GetComponent<Rigidbody2D>();
        // Asegúrate de que Gravity Scale sea 0 en el Inspector
    }

    private void Update()
    {
        if (!estaVivo) return;

        HandleEntradas();

        // Lógica de disparo con Cooldown
        if (Input.GetKey(KeyCode.Space) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    private void FixedUpdate()
    {
        if (!estaVivo) return;

        // Aplicar aceleración
        if (estaAcelerando)
        {
            naveRigidbody.AddForce(transform.up * aceleracionNave);
        }

        // Limitar velocidad máxima (Evita el error de obsoleto)
        if (naveRigidbody.linearVelocity.sqrMagnitude > maximaVelocidad * maximaVelocidad)
        {
            naveRigidbody.linearVelocity = naveRigidbody.linearVelocity.normalized * maximaVelocidad;
        }

        // Aplicar rotación
        float rotacion = direccionRotacion * rotacionVelocidad * Time.fixedDeltaTime;
        naveRigidbody.MoveRotation(naveRigidbody.rotation + rotacion);
    }

    private void HandleEntradas()
    {
        // Aceleración (W o Flecha Arriba)
        estaAcelerando = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

        // Rotación (A/D o Flechas Izq/Der)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            direccionRotacion = 1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            direccionRotacion = -1f;
        else
            direccionRotacion = 0f;
    }

    private void Disparar()
    {
        if (balaPrefab != null && puntoDisparo != null)
        {
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
        }
    }
}