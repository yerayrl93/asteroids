using UnityEngine;
using System.Collections;

public class EnemigoNave : MonoBehaviour
{
    public Transform jugador;
    public Transform puntoDisparo;

    [Header("Configuración Visual")]
    public GameObject efectoExplosion;

    [Header("Ajustes")]
    public float velocidad = 3f;
    public float tiempoEntreDisparos = 2f;
    public int puntosAlMorir = 500;
    private float cronometroDisparo;
    private bool estaMuriendo = false;

    void Start()
    {
        // Buscamos al jugador por Tag si no está asignado
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null || estaMuriendo) return;

        // Movimiento suave hacia el jugador
        transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);

        // Rotación para mirar al jugador
        Vector2 direccion = (Vector2)jugador.position - (Vector2)transform.position;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Lógica de disparo
        cronometroDisparo += Time.deltaTime;
        if (cronometroDisparo >= tiempoEntreDisparos)
        {
            Disparar();
            cronometroDisparo = 0;
        }
    }

    void Disparar()
    {
        if (estaMuriendo) return;

        // Uso del Pool de balas enemigas
        if (BalaEnemigaPool.Instance != null && puntoDisparo != null)
        {
            GameObject bala = BalaEnemigaPool.Instance.GetBalaEnemiga();
            if (bala != null)
            {
                bala.transform.position = puntoDisparo.position;
                bala.transform.rotation = puntoDisparo.rotation;
                bala.SetActive(true);

                Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
                if (rbBala != null) rbBala.linearVelocity = puntoDisparo.up * 8f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMuriendo) return;

        // Si nos da una bala del jugador
        if (other.CompareTag("Bala"))
        {
            if (GameManager.Instance != null) GameManager.Instance.GanarPuntos(puntosAlMorir);
            other.gameObject.SetActive(false); // Desactivamos la bala (Pooling)
            StartCoroutine(SecuenciaMuerte());
        }

        // Si chocamos contra el jugador
        if (other.CompareTag("Player"))
        {
            Jugador scriptJugador = other.GetComponent<Jugador>();
            if (scriptJugador != null) scriptJugador.TomarDaño();
            StartCoroutine(SecuenciaMuerte());
        }
    }

    IEnumerator SecuenciaMuerte()
    {
        estaMuriendo = true;

        // Desactivamos visuales y colisiones para que no siga interactuando mientras explota
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Efecto de explosiones encadenadas
        for (int i = 0; i < 3; i++)
        {
            if (efectoExplosion != null)
            {
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Instantiate(efectoExplosion, transform.position + offset, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // --- SOLTAR BUFF DESDE EL POOL (25% Probabilidad) ---
        if (Random.Range(0f, 100f) <= 25f)
        {
            if (BuffPool.Instance != null)
            {
                // Decidimos qué soltar: 10% de probabilidad de Vida, 90% Cadencia
                bool esVida = Random.Range(0, 100) < 10;

                // Llamamos al método pasándole el booleano obligatorio
                GameObject buff = BuffPool.Instance.GetBuff(esVida);

                if (buff != null)
                {
                    buff.transform.position = transform.position;
                    buff.SetActive(true);
                }
            }
        }

        if (GameManager.Instance != null) GameManager.Instance.CheckNivelCompletado();

        // Como la nave no está en un Pool (según tu código), usamos Destroy
        Destroy(gameObject);
    }
}