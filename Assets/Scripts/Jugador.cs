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

    [Header("Salud")]
    [SerializeField] private int vidas = 3; // Ajustado a 3 para dificultad estándar
    [SerializeField] private GameObject efectoExplosion; // Arrastra un sistema de partículas aquí

    private Rigidbody2D naveRigidbody;
    private bool estaVivo = true;
    private bool estaAcelerando = false;
    private float direccionRotacion = 0f;
    private float tiempoProximoDisparo;

    private void Start()
    {
        naveRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!estaVivo) return;

        HandleEntradas();

        if (Input.GetKey(KeyCode.Space) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    private void FixedUpdate()
    {
        if (!estaVivo) return;

        if (estaAcelerando)
        {
            naveRigidbody.AddForce(transform.up * aceleracionNave);
        }

        if (naveRigidbody.linearVelocity.sqrMagnitude > maximaVelocidad * maximaVelocidad)
        {
            naveRigidbody.linearVelocity = naveRigidbody.linearVelocity.normalized * maximaVelocidad;
        }

        float rotacion = direccionRotacion * rotacionVelocidad * Time.fixedDeltaTime;
        naveRigidbody.MoveRotation(naveRigidbody.rotation + rotacion);
    }

    private void HandleEntradas()
    {
        estaAcelerando = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

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

    // --- NUEVAS FUNCIONES DE DAÑO Y MUERTE ---

    public void TomarDaño()
    {
        if (!estaVivo) return;

        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            Muerte();
        }
        else
        {
            ResetearPosicion();
        }
    }

    private void ResetearPosicion()
    {
        // Pequeño efecto visual al chocar (opcional)
        if (efectoExplosion != null)
        {
            Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        }

        transform.position = Vector3.zero;
        naveRigidbody.linearVelocity = Vector2.zero;
        naveRigidbody.angularVelocity = 0f;
    }

    private void Muerte()
    {
        estaVivo = false;

        // Efecto de explosión final
        if (efectoExplosion != null)
        {
            Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
        Debug.Log("Game Over");

        // LLAMADA AL GAMEMANAGER: Esto guarda los puntos y cambia a la escena "Final"
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Morir();
        }
        else
        {
            Debug.LogError("Falta el GameManager en la escena para procesar la muerte.");
        }
    }
}