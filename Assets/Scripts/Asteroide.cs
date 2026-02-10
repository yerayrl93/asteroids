using UnityEngine;

public class Asteroide : MonoBehaviour
{
    private Rigidbody2D rb;
    public int nivel = 3;
    public float velocidadBase = 2f;

    [Header("Configuración Oro")]
    public bool esOro = false;
    public int multiplicadorOro = 5;

    [HideInInspector] public float velocidadDificultad = 1f;

    // Límites para el Wrap
    private float limiteX;
    private float limiteY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculamos los límites igual que en el jugador
        Camera cam = Camera.main;
        limiteY = cam.orthographicSize;
        limiteX = limiteY * cam.aspect;
    }

    private void OnEnable()
    {
        // Dirección aleatoria al activarse
        float angulo = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Ajuste visual según nivel y si es Oro
        if (esOro)
        {
            transform.localScale = Vector3.one * (nivel * 0.08f);
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            transform.localScale = Vector3.one * (nivel * 0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        float multiplicadorGlobal = (GameManager.Instance != null) ? GameManager.Instance.multiplicadorVelocidad : 1f;
        float extraOro = esOro ? 1.5f : 1f;

        rb.linearVelocity = transform.up * (velocidadBase * multiplicadorGlobal * velocidadDificultad * extraOro);
    }

    void Update()
    {
        ScreenWrap();
    }

    private void ScreenWrap()
    {
        Vector3 pos = transform.position;
        float margen = 0.6f; // Un poco más que el jugador por el tamaño de los asteroides grandes

        if (pos.x > limiteX + margen) pos.x = -limiteX - margen;
        else if (pos.x < -limiteX - margen) pos.x = limiteX + margen;

        if (pos.y > limiteY + margen) pos.y = -limiteY - margen;
        else if (pos.y < -limiteY - margen) pos.y = limiteY + margen;

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con bala del jugador, bala enemiga o el propio jugador
        if (other.CompareTag("Bala") || other.CompareTag("Player") || other.CompareTag("BalaEnemigo"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Jugador>().TomarDaño();
            }

            if (GameManager.Instance != null)
            {
                int puntosAOtorgar = (4 - nivel) * 100;
                if (esOro) puntosAOtorgar *= multiplicadorOro;
                GameManager.Instance.GanarPuntos(puntosAOtorgar);
            }

            if (!esOro) Dividir();

            // En lugar de destruir, desactivamos (Object Pool)
            gameObject.SetActive(false);
            GameManager.Instance.CheckNivelCompletado();
        }
    }

    private void Dividir()
    {
        if (nivel <= 1) return;

        for (int i = 0; i < 2; i++) // Cambia a 3 si quieres más fragmentos
        {
            GameObject clon = AsteroidePool.Instance.GetAsteroide();
            if (clon != null)
            {
                clon.transform.position = transform.position;
                Asteroide scriptClon = clon.GetComponent<Asteroide>();

                scriptClon.nivel = this.nivel - 1;
                scriptClon.esOro = false;
                scriptClon.velocidadDificultad = this.velocidadDificultad;

                clon.SetActive(true);
            }
        }
    }
}