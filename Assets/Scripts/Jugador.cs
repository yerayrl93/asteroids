using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jugador : MonoBehaviour
{
    // ... Tus variables anteriores se mantienen igual ...
    [Header("Parametros Nave")]
    [SerializeField] private float aceleracionNave = 10f;
    [SerializeField] private float maximaVelocidad = 10f;
    [SerializeField] private float rotacionVelocidad = 180f;

    [Header("Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerable = 2f;
    [SerializeField] private float velocidadParpadeo = 0.1f;
    private bool esInvulnerable = false;
    private SpriteRenderer spriteRenderer;

    [Header("Salud")]
    [SerializeField] private int vidas = 3;
    [SerializeField] private GameObject efectoExplosion;

    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 0.3f;

    private Rigidbody2D naveRigidbody;
    private bool estaVivo = true;
    private bool estaAcelerando = false;
    private float direccionRotacion = 0f;
    private float tiempoProximoDisparo;

    private void Start() => naveRigidbody = GetComponent<Rigidbody2D>();

    private void Update()
    {
        if (!estaVivo) return;
        HandleEntradas();
        if (Input.GetKey(KeyCode.Space) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    private void FixedUpdate()
    {
        if (!estaVivo) return;
        if (estaAcelerando) naveRigidbody.AddForce(transform.up * aceleracionNave);

        if (naveRigidbody.linearVelocity.sqrMagnitude > maximaVelocidad * maximaVelocidad)
            naveRigidbody.linearVelocity = naveRigidbody.linearVelocity.normalized * maximaVelocidad;

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

    private void Disparar()
    {
        if (balaPrefab != null && puntoDisparo != null)
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
    }

    public void TomarDaño()
    {
        if (!estaVivo || esInvulnerable) return;

        vidas--;
        if (VidaPool.Instance != null) VidaPool.Instance.RestarVidaVisual();

        if (vidas <= 0) Muerte();
        else
        {
            StartCoroutine(ActivarEscudo());
            ResetearPosicion();
        }
    }

    private IEnumerator ActivarEscudo()
    {
        esInvulnerable = true;
        float tiempoPasado = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();

        while (tiempoPasado < tiempoInvulnerable)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(velocidadParpadeo);
            tiempoPasado += velocidadParpadeo;
        }

        spriteRenderer.enabled = true;
        esInvulnerable = false;
    }

    // --- AQUÍ ESTÁN LAS MODIFICACIONES DE COLISIÓN ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Detectar Bala Enemiga
        if (other.CompareTag("BalaEnemigo"))
        {
            TomarDaño();
            Destroy(other.gameObject); // Destruimos la bala que nos pegó
        }

        // 2. Detectar Nave Enemiga (Colisión física directa)
        if (other.CompareTag("EnemigoNave"))
        {
            TomarDaño();
            // Opcional: Destruir la nave enemiga al chocar con ella
            Destroy(other.gameObject);
        }

        // 3. Detectar Vida Extra
        if (other.CompareTag("VidaExtra"))
        {
            vidas++;
            if (VidaPool.Instance != null) VidaPool.Instance.SumarVidaVisual();
            Destroy(other.gameObject);
            Debug.Log("Vidas actuales: " + vidas);
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