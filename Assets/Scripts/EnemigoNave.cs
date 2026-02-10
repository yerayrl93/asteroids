using UnityEngine;

public class EnemigoNave : MonoBehaviour
{
    public Transform jugador;
    public GameObject balaEnemigoPrefab;
    public Transform puntoDisparo;

    [Header("Configuración Visual")]
    public GameObject efectoExplosion;

    [Header("Ajustes")]
    public float velocidad = 3f;
    public float tiempoEntreDisparos = 2f;
    public int puntosAlMorir = 500;
    private float cronometroDisparo;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
        Vector2 direccion = (Vector2)jugador.position - (Vector2)transform.position;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        cronometroDisparo += Time.deltaTime;
        if (cronometroDisparo >= tiempoEntreDisparos)
        {
            Disparar();
            cronometroDisparo = 0;
        }
    }

    void Disparar()
    {
        if (balaEnemigoPrefab != null && puntoDisparo != null)
        {
            GameObject bala = Instantiate(balaEnemigoPrefab, puntoDisparo.position, puntoDisparo.rotation);
            Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
            if (rbBala != null) rbBala.linearVelocity = puntoDisparo.up * 8f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BalaEnemigo")) return;

        if (other.CompareTag("Bala"))
        {
            if (GameManager.Instance != null) GameManager.Instance.GanarPuntos(puntosAlMorir);
            Destroy(other.gameObject);
            MuerteEnemigo();
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Jugador>().TomarDaño();
            MuerteEnemigo();
        }
    }

    void MuerteEnemigo()
    {
        if (efectoExplosion != null)
        {
            Instantiate(efectoExplosion, transform.position, transform.rotation);
        }

        // --- AVISO AL GAMEMANAGER ---
        if (GameManager.Instance != null)
            GameManager.Instance.CheckNivelCompletado();

        Destroy(gameObject);
    }
}