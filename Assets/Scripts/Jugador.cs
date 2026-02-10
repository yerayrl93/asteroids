using UnityEngine;
using System.Collections;

public class Jugador : MonoBehaviour
{
    [Header("Parámetros Nave")]
    [SerializeField] private float aceleracionNave = 10f;
    [SerializeField] private float maximaVelocidad = 10f;
    [SerializeField] private float rotacionVelocidad = 180f;

    [Header("Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerableDano = 2f;
    [SerializeField] private float tiempoInvulnerableNivel = 3f;
    [SerializeField] private float velocidadParpadeo = 0.1f;
    private bool esInvulnerable = false;
    private SpriteRenderer spriteRenderer;

    [Header("Salud")]
    [SerializeField] private int vidas = 3;
    [SerializeField] private GameObject efectoExplosion;

    [Header("Ajustes Disparo")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 0.3f;

    private Rigidbody2D naveRigidbody;
    private bool estaVivo = true;
    private bool estaAcelerando = false;
    private float direccionRotacion = 0f;
    private float tiempoProximoDisparo;

    // Límites de pantalla para el Wrap Around
    private float limiteX;
    private float limiteY;

    private void Start()
    {
        naveRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calcular límites de pantalla automáticamente basados en la cámara principal
        Camera cam = Camera.main;
        limiteY = cam.orthographicSize; // Mitad de la altura
        limiteX = limiteY * cam.aspect; // Mitad de la anchura

        // Escudo inicial al spawnear (Nivel 1)
        ActivarEscudoTemporal(tiempoInvulnerableNivel);
    }

    private void Update()
    {
        if (!estaVivo) return;

        HandleEntradas();
        ScreenWrap(); // Aplicar el efecto Asteroids

        if (Input.GetKey(KeyCode.Space) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    private void FixedUpdate()
    {
        if (!estaVivo) return;

        // Movimiento
        if (estaAcelerando) naveRigidbody.AddForce(transform.up * aceleracionNave);

        // Limitar velocidad
        if (naveRigidbody.linearVelocity.sqrMagnitude > maximaVelocidad * maximaVelocidad)
            naveRigidbody.linearVelocity = naveRigidbody.linearVelocity.normalized * maximaVelocidad;

        // Rotación
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

    private void ScreenWrap()
    {
        Vector3 pos = transform.position;

        // Si sale por derecha/izquierda (añadimos un pequeño margen de 0.5f)
        if (pos.x > limiteX + 0.5f) pos.x = -limiteX - 0.5f;
        else if (pos.x < -limiteX - 0.5f) pos.x = limiteX + 0.5f;

        // Si sale por arriba/abajo
        if (pos.y > limiteY + 0.5f) pos.y = -limiteY - 0.5f;
        else if (pos.y < -limiteY - 0.5f) pos.y = limiteY + 0.5f;

        transform.position = pos;
    }

    private void Disparar()
    {
        if (BalaPool.Instance != null)
        {
            GameObject bala = BalaPool.Instance.GetBala();
            if (bala != null)
            {
                bala.transform.position = puntoDisparo.position;
                bala.transform.rotation = puntoDisparo.rotation;
                bala.SetActive(true);
            }
        }
    }

    public void TomarDaño()
    {
        if (!estaVivo || esInvulnerable) return;

        vidas--;
        if (VidaPool.Instance != null) VidaPool.Instance.RestarVidaVisual();

        if (vidas <= 0) Muerte();
        else
        {
            ResetearPosicion();
            ActivarEscudoTemporal(tiempoInvulnerableDano);
        }
    }

    public void ActivarEscudoTemporal(float duracion)
    {
        StopCoroutine("EfectoParpadeo");
        StartCoroutine(EfectoParpadeo(duracion));
    }

    private IEnumerator EfectoParpadeo(float duracion)
    {
        esInvulnerable = true;
        float tiempoPasado = 0;

        while (tiempoPasado < duracion)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(velocidadParpadeo);
            tiempoPasado += velocidadParpadeo;
        }

        spriteRenderer.enabled = true;
        esInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (esInvulnerable) return;

        if (other.CompareTag("BalaEnemigo") || other.CompareTag("EnemigoNave") || other.CompareTag("Asteroide"))
        {
            TomarDaño();

            // Si es bala enemiga, desactivarla (Pool)
            if (other.CompareTag("BalaEnemigo")) other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("VidaExtra"))
        {
            vidas++;
            if (VidaPool.Instance != null) VidaPool.Instance.SumarVidaVisual();
            Destroy(other.gameObject);
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