using UnityEngine;
using System.Collections;

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
    private bool estaMuriendo = false; // Evita errores de doble muerte

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null || estaMuriendo) return;

        // Movimiento y rotación hacia el jugador
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
        if (estaMuriendo) return;
        if (balaEnemigoPrefab != null && puntoDisparo != null)
        {
            GameObject bala = Instantiate(balaEnemigoPrefab, puntoDisparo.position, puntoDisparo.rotation);
            Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
            if (rbBala != null) rbBala.linearVelocity = puntoDisparo.up * 8f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMuriendo) return;
        if (other.CompareTag("BalaEnemigo")) return;

        if (other.CompareTag("Bala"))
        {
            if (GameManager.Instance != null) GameManager.Instance.GanarPuntos(puntosAlMorir);

            // --- SOLUCIÓN AL BUG DE LAS BALAS ---
            // En lugar de Destroy, desactivamos la bala para que vuelva al Pool
            other.gameObject.SetActive(false);

            StartCoroutine(SecuenciaMuerte());
        }

        if (other.CompareTag("Player"))
        {
            Jugador scriptJugador = other.GetComponent<Jugador>();
            if (scriptJugador != null) scriptJugador.TomarDaño();
            StartCoroutine(SecuenciaMuerte());
        }
    }

    // --- SECUENCIA DE EXPLOSIONES Y MUERTE SEGURA ---
    IEnumerator SecuenciaMuerte()
    {
        estaMuriendo = true; // Bloquea el Update y disparos

        // Desactivamos el sprite y el collider para que no "exista" físicamente
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Bucle de explosiones (3 explosiones con ligero desfase)
        for (int i = 0; i < 3; i++)
        {
            if (efectoExplosion != null)
            {
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Instantiate(efectoExplosion, transform.position + offset, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // Aviso al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.CheckNivelCompletado();

        // Finalmente, eliminamos el objeto
        Destroy(gameObject);
    }
}