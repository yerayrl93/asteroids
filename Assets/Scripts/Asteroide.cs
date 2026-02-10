using UnityEngine;

public class Asteroide : MonoBehaviour
{
    private Rigidbody2D rb;
    public int nivel = 3;
    public float velocidadBase = 2f;

    [Header("Configuración Oro")]
    public bool esOro = false; // Se marca por código o en el Inspector
    public int multiplicadorOro = 5;

    [HideInInspector] public float velocidadDificultad = 1f;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()
    {
        float angulo = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Si es oro, lo hacemos un poco más pequeño y brillante
        if (esOro)
        {
            transform.localScale = Vector3.one * (nivel * 0.08f);
            GetComponent<SpriteRenderer>().color = Color.yellow; // Color dorado
        }
        else
        {
            transform.localScale = Vector3.one * (nivel * 0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        float multiplicadorGlobal = (GameManager.Instance != null) ? GameManager.Instance.multiplicadorVelocidad : 1f;

        // El de oro va un 50% más rápido que uno normal
        float extraOro = esOro ? 1.5f : 1f;
        rb.linearVelocity = transform.up * (velocidadBase * multiplicadorGlobal * velocidadDificultad * extraOro);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bala") || other.CompareTag("Player") || other.CompareTag("BalaEnemigo"))
        {
            if (other.CompareTag("Player"))
            {
                Jugador jugador = other.GetComponent<Jugador>();
                if (jugador != null) jugador.TomarDaño();
            }

            if (GameManager.Instance != null)
            {
                int puntosAOtorgar = (4 - nivel) * 100;
                if (esOro) puntosAOtorgar *= multiplicadorOro; // ¡Muchos más puntos!

                GameManager.Instance.GanarPuntos(puntosAOtorgar);
            }

            // El de oro no se divide para que sea una pieza única valiosa
            if (!esOro) Dividir();

            gameObject.SetActive(false);
            GameManager.Instance.CheckNivelCompletado();
        }

        if (other.CompareTag("Limite"))
        {
            rb.linearVelocity = -rb.linearVelocity;
            transform.up = rb.linearVelocity.normalized;
            Vector2 direccionAlCentro = ((Vector2)Vector3.zero - (Vector2)transform.position).normalized;
            transform.position += (Vector3)direccionAlCentro * 0.5f;
        }
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, Vector2.zero) > 25f)
        {
            gameObject.SetActive(false);
            GameManager.Instance.CheckNivelCompletado();
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
                scriptClon.esOro = false; // Los hijos no son de oro
                scriptClon.velocidadDificultad = this.velocidadDificultad;

                clon.SetActive(true);
            }
        }
    }
}