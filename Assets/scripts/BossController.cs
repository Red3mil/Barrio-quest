using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int vida = 3;

    [Header("Ataque")]
    public float rangoMinimoDistancia = 1.0f; // NUEVO: Distancia mínima que mantiene del jugador
    public float tiempoEntreAtaques = 1.5f;
    public float rangoAtaque = 1.5f;  // Distancia para atacar

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    public bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo = true;
    private Animator animator;
    private Vector3 escalaOriginal;
    private moverPersonaje playerScript;
    private float ultimoAtaqueTiempo = -999f;

    private bool atacando;  // Flag para saber si está atacando

    public float fuerzaRebote = 5f;  // Para rebote controlado

    void Start()
    {
        if (player == null)
        {
            GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
            if (jugadorGO != null)
            {
                player = jugadorGO.transform;
                playerScript = jugadorGO.GetComponent<moverPersonaje>();
            }
        }
        else
        {
            playerScript = player.GetComponent<moverPersonaje>();
        }

        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;

        // Ignorar colisiones físicas con el jugador
        Collider2D enemigoCollider = GetComponent<Collider2D>();
        Collider2D jugadorCollider = player != null ? player.GetComponent<Collider2D>() : null;
        if (enemigoCollider != null && jugadorCollider != null)
            Physics2D.IgnoreCollision(enemigoCollider, jugadorCollider, true);

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (playerScript != null && playerScript.vida <= 0)
        {
            enMovimiento = false;
            playerVivo = false;
        }
        else if (playerVivo && !muerto)
        {
            if (!atacando)  // Solo moverse si NO está atacando
                Movimiento();

            RevisarAtaquePorDistancia();
        }
        else
        {
            enMovimiento = false;
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerte", muerto);
        // No uses SetBool para 'atacar', porque es trigger
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ÚNICO CAMBIO: Agregado "&& distanceToPlayer > rangoMinimoDistancia"
        if (distanceToPlayer < detectionRadius && distanceToPlayer > rangoMinimoDistancia && playerVivo && !muerto)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Mirar al jugador
            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            else if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

            movement = new Vector2(direction.x, direction.y);

            if (!recibiendoDanio)
            {
                rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
            }

            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }
    }

    private void RevisarAtaquePorDistancia()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= rangoAtaque && Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques && !muerto && playerVivo)
        {
            animator.SetTrigger("atacar");  // Trigger para ataque
            atacando = true;
            ultimoAtaqueTiempo = Time.time;
        }
    }

    // Llamado desde Animation Event al final de la animación de ataque
    public void TerminarAtaque()
    {
        atacando = false;
    }

    // Llamado desde Animation Event en el frame de daño
    public void AplicarDanio()
    {
        if (muerto) return;

        if (playerScript != null && !playerScript.muerto && !playerScript.recibiendoDanio)
        {
            Vector2 direccionDanio = (player.position - transform.position).normalized;
            playerScript.RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio && !muerto)
        {
            vida -= cantDanio;
            recibiendoDanio = true;
            animator.SetTrigger("recibirDanio");

            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;
                animator.SetTrigger("muerte");
                rb.linearVelocity = Vector2.zero;
                StartCoroutine(EliminarDespuesDeSegundos(2f));
            }
            else
            {
                // Rebote controlado sin usar AddForce
                Vector2 rebote = -direccion.normalized * fuerzaRebote * 0.1f;
                rb.MovePosition(rb.position + rebote);

                StartCoroutine(Blink());
                StartCoroutine(ResetDanio());
            }
        }
    }

    IEnumerator ResetDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator Blink()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        for (int i = 0; i < 4; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FadeOut()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color colorOriginal = sr.color;
        float duracion = 1f;
        float t = 0f;

        while (t < duracion)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duracion);
            sr.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
    }

    IEnumerator EliminarDespuesDeSegundos(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        yield return StartCoroutine(FadeOut());
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // NUEVO: Visualizar distancia mínima
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoMinimoDistancia);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}