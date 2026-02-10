using UnityEngine;
using System.Collections;

public class EnemigoNave : MonoBehaviour
{
    public Transform jugador;
    public Transform puntoDisparo;

    [Header("Configuración Visual")]
    public GameObject efectoExplosion;

    [Header("Referencias Power-Ups")]
    public GameObject prefabBateriaBuff; // Se asigna desde el Spawner automáticamente

    [Header("Ajustes")]
    public float velocidad = 3f;
    public float tiempoEntreDisparos = 2f;
    public int puntosAlMorir = 500;
    private float cronometroDisparo;
    private bool estaMuriendo = false;

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

        if (other.CompareTag("Bala"))
        {
            if (GameManager.Instance != null) GameManager.Instance.GanarPuntos(puntosAlMorir);
            other.gameObject.SetActive(false); // Desactivar bala del jugador
            StartCoroutine(SecuenciaMuerte());
        }

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

        // Desactivamos visuales y colisiones inmediatamente
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Efecto de varias explosiones
        for (int i = 0; i < 3; i++)
        {
            if (efectoExplosion != null)
            {
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Instantiate(efectoExplosion, transform.position + offset, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.1f);
        }

        // --- SOLTAR BUFF (25% Probabilidad) ---
        if (prefabBateriaBuff != null)
        {
            float probabilidadSuelto = Random.Range(0f, 100f);
            if (probabilidadSuelto <= 25f) // Cambiado a 25%
            {
                // Forzamos Z=0 para evitar problemas de visibilidad
                Vector3 posSpawn = new Vector3(transform.position.x, transform.position.y, 0f);
                Instantiate(prefabBateriaBuff, posSpawn, Quaternion.identity);
            }
        }

        if (GameManager.Instance != null) GameManager.Instance.CheckNivelCompletado();

        Destroy(gameObject);
    }
}