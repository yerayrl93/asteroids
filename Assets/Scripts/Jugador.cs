using UnityEngine;

public class Jugador : MonoBehaviour
{
    [Header("Parametros Nave")]
    [SerializeField] private float aceleracionNave = 10f;
    [SerializeField] private float maximaVelocidad = 10f;
    [SerializeField] private float rotacionVelocidad = 180f;



    [Header("Salud")]
    [SerializeField] private int vidas = 3;
    [SerializeField] private GameObject efectoExplosion;

    // ... (El resto de tus variables de disparo y movimiento se mantienen igual) ...

    private Rigidbody2D naveRigidbody;
    private bool estaVivo = true;
    private bool estaAcelerando = false;
    private float direccionRotacion = 0f;
    private float tiempoProximoDisparo;

    // Referencias que faltaban en tu código original para que compile bien:
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 0.3f;

    private void Start() => naveRigidbody = GetComponent<Rigidbody2D>();

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
        if (estaAcelerando) naveRigidbody.AddForce(transform.up * aceleracionNave);

        // Limitar velocidad
        if (naveRigidbody.linearVelocity.sqrMagnitude > maximaVelocidad * maximaVelocidad)
            naveRigidbody.linearVelocity = naveRigidbody.linearVelocity.normalized * maximaVelocidad;

        float rotacion = direccionRotacion * rotacionVelocidad * Time.fixedDeltaTime;
        naveRigidbody.MoveRotation(naveRigidbody.rotation + rotacion);
    }

    private void HandleEntradas()
    {
        estaAcelerando = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) direccionRotacion = 1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) direccionRotacion = -1f;
        else direccionRotacion = 0f;
    }

    private void Disparar()
    {
        if (balaPrefab != null && puntoDisparo != null)
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
    }

    public void TomarDaño()
    {
        if (!estaVivo) return;
        vidas--;
        if (VidaPool.Instance != null) VidaPool.Instance.RestarVidaVisual();

        if (vidas <= 0) Muerte();
        else ResetearPosicion();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VidaExtra"))
        {
            if (other.CompareTag("VidaExtra"))
            {
                vidas++; // Sube la variable lógica

                if (VidaPool.Instance != null)
                {
                    VidaPool.Instance.SumarVidaVisual(); // Llama a la nueva lógica
                }

                Destroy(other.gameObject); // Destruye el objeto del mapa
                Debug.Log("Vidas actuales: " + vidas);
            }
        }
    }

    private void ResetearPosicion()
    {
        if (efectoExplosion != null) Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        transform.position = Vector3.zero;
        naveRigidbody.linearVelocity = Vector2.zero;
        naveRigidbody.angularVelocity = 0f;
    }

    private void Muerte()
    {
        estaVivo = false;
        if (efectoExplosion != null) Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.Morir();
    }
}