using UnityEngine;

public class Asteroide : MonoBehaviour
{
    private Rigidbody2D rb;
    public int nivel = 3;
    public float velocidadBase = 2f;

    [Header("Configuración Oro")]
    public bool esOro = false;
    public int multiplicadorOro = 5;
    public GameObject prefabBateriaBuff;

    [HideInInspector] public float velocidadDificultad = 1f;

    // Límites para el Wrap
    private float limiteX;
    private float limiteY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculamos los límites basados en la cámara
        Camera cam = Camera.main;
        limiteY = cam.orthographicSize;
        limiteX = limiteY * cam.aspect;
    }

    private void OnEnable()
    {
        // Dirección aleatoria al activarse del Pool
        float angulo = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Ajuste visual y de tamaño según si es Oro o Normal
        if (esOro)
        {
            transform.localScale = Vector3.one * (nivel * 0.08f);
            GetComponent<SpriteRenderer>().color = Color.yellow; // Asegura que brille amarillo
        }
        else
        {
            transform.localScale = Vector3.one * (nivel * 0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        float multiplicadorGlobal = (GameManager.Instance != null) ? GameManager.Instance.multiplicadorVelocidad : 1f;
        float extraOro = esOro ? 1.5f : 1f;

        // Aplicamos velocidad al Rigidbody
        rb.linearVelocity = transform.up * (velocidadBase * multiplicadorGlobal * velocidadDificultad * extraOro);
    }

    void Update()
    {
        ScreenWrap();
    }

    private void ScreenWrap()
    {
        Vector3 pos = transform.position;
        float margen = 0.6f;

        if (pos.x > limiteX + margen) pos.x = -limiteX - margen;
        else if (pos.x < -limiteX - margen) pos.x = limiteX + margen;

        if (pos.y > limiteY + margen) pos.y = -limiteY - margen;
        else if (pos.y < -limiteY - margen) pos.y = limiteY + margen;

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectar colisión con balas (jugador/enemigo) o el jugador directamente
        if (other.CompareTag("Bala") || other.CompareTag("Player") || other.CompareTag("BalaEnemigo"))
        {
            if (other.CompareTag("Player"))
            {
                Jugador scriptJugador = other.GetComponent<Jugador>();
                if (scriptJugador != null) scriptJugador.TomarDaño();
            }

            // Gestión de puntos
            if (GameManager.Instance != null)
            {
                int puntosAOtorgar = (4 - nivel) * 100;
                if (esOro) puntosAOtorgar *= multiplicadorOro;
                GameManager.Instance.GanarPuntos(puntosAOtorgar);
            }

            // --- LÓGICA EXCLUSIVA DEL DORADO ---
            if (esOro)
            {
                if (prefabBateriaBuff != null)
                {
                    // Forzamos Z=0 para que el buff sea visible
                    Vector3 posSpawn = new Vector3(transform.position.x, transform.position.y, 0f);
                    Instantiate(prefabBateriaBuff, posSpawn, Quaternion.identity);
                }
                // El dorado no se divide, muere directamente
            }
            else
            {
                // El normal se divide en trozos más pequeños
                Dividir();
            }

            // Regreso al Pool y aviso al GameManager
            gameObject.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.CheckNivelCompletado();
        }
    }

    private void Dividir()
    {
        if (nivel <= 1) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject clon = AsteroidePool.Instance.GetAsteroide();
            if (clon != null)
            {
                clon.transform.position = transform.position;
                Asteroide scriptClon = clon.GetComponent<Asteroide>();

                scriptClon.nivel = this.nivel - 1;
                scriptClon.esOro = false; // Los hijos de un normal siempre son normales
                scriptClon.velocidadDificultad = this.velocidadDificultad;

                clon.SetActive(true);
            }
        }
    }
}