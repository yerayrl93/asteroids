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

    // --- VARIABLES PARA EL BUFF ---
    private float tiempoOriginalDisparo;
    private bool tieneBuffCadencia = false;
    // ------------------------------

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

        // Guardamos la cadencia original para poder volver a ella tras el buff
        tiempoOriginalDisparo = tiempoEntreDisparos;

        // Calcular límites de pantalla automáticamente basados en la cámara principal
        Camera cam = Camera.main;
        limiteY = cam.orthographicSize;
        limiteX = limiteY * cam.aspect;

        // Escudo inicial al spawnear
        ActivarEscudoTemporal(tiempoInvulnerableNivel);
    }

    private void Update()
    {
        if (!estaVivo) return;

        HandleEntradas();
        ScreenWrap();

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
        if (pos.x > limiteX + 0.5f) pos.x = -limiteX - 0.5f;
        else if (pos.x < -limiteX - 0.5f) pos.x = limiteX + 0.5f;

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

    // --- LÓGICA DEL BUFF ---
    public void AplicarBuffCadencia(float duracion)
    {
        StartCoroutine(RutinaBuff(duracion));
    }

    private IEnumerator RutinaBuff(float duracion)
    {
        tieneBuffCadencia = true;
        // Reducimos el tiempo entre disparos a la mitad (dispara el doble de rápido)
        tiempoEntreDisparos = tiempoOriginalDisparo / 2f;

        // Feedback visual: Nave Amarilla
        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(duracion);

        // Volver a la normalidad
        tiempoEntreDisparos = tiempoOriginalDisparo;
        spriteRenderer.color = Color.white;
        tieneBuffCadencia = false;
    }
    // -----------------------

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
        // Si justo termina la invulnerabilidad pero seguimos con el Buff, nos aseguramos de ser amarillos
        if (tieneBuffCadencia) spriteRenderer.color = Color.yellow;
        else spriteRenderer.color = Color.white;

        esInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Detección de Buff de Cadencia (Batería)
        if (other.CompareTag("BuffCadencia"))
        {
            AplicarBuffCadencia(5f); // 5 segundos de super disparo
            Destroy(other.gameObject);
            return; // Salimos para no procesar daño si el buff toca a la vez que un enemigo
        }

        if (esInvulnerable) return;

        if (other.CompareTag("BalaEnemigo") || other.CompareTag("EnemigoNave") || other.CompareTag("Asteroide"))
        {
            TomarDaño();
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