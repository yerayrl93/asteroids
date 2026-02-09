using UnityEngine;

public class Asteroide : MonoBehaviour
{
    private Rigidbody2D rb;
    public int nivel = 3; // 3 = Grande, 2 = Mediano, 1 = Pequeño
    public float velocidadBase = 2f;

    [HideInInspector] public float velocidadDificultad = 1f;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()
    {
        // 1. Dirección aleatoria
        float angulo = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // 2. Escala visual proporcional al nivel
        transform.localScale = Vector3.one * (nivel * 0.1f);

        // 3. Aplicar velocidad combinada
        float multiplicadorGlobal = (GameManager.Instance != null) ? GameManager.Instance.multiplicadorVelocidad : 1f;
        rb.linearVelocity = transform.up * (velocidadBase * multiplicadorGlobal * velocidadDificultad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // COLISIÓN CON BALA O JUGADOR
        if (other.CompareTag("Bala") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player"))
            {
                Jugador jugador = other.GetComponent<Jugador>();
                if (jugador != null) jugador.TomarDaño();
            }

            // --- LÓGICA DE PUNTOS ---
            if (GameManager.Instance != null)
            {
                // Damos puntos según el nivel (Ej: Grande 100, Mediano 200, Pequeño 300)
                int puntosAOtorgar = (4 - nivel) * 100;
                GameManager.Instance.GanarPuntos(puntosAOtorgar);
            }

            Dividir();
            gameObject.SetActive(false);
            GameManager.Instance.CheckNivelCompletado();
        }

        // REBOTE EN LÍMITES
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
            Debug.Log("Asteroide fugitivo capturado.");
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
                scriptClon.velocidadDificultad = this.velocidadDificultad;

                clon.SetActive(true);
            }
        }
    }
}