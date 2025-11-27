using UnityEngine;
using System.Collections;

public class AmetBoss : MonoBehaviour
{
    [Header("Referencias")]
    public Transform attackPoint;
    public LayerMask playerLayer;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D bossCollider;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Movimiento")]
    public float speed = 2.5f;
    public float detectionRadius = 10f;
    public float rangoMinimoDistancia = 0.8f; // NUEVO: Distancia mínima que mantiene del jugador
    public float stopRange = 1.1f;

    [Header("Vida y Daño")]
    public int vidaMaxima = 12;
    public int vida = 12;

    [Header("Ataques")]
    public float intervaloAtaque = 1.2f;
    public float rangoAtaque1 = 1.4f;
    public int danoAtaque1 = 1;
    public float rangoAtaque2 = 1.8f;
    public int danoAtaque2 = 1;
    public float rangoAtaque3 = 2.2f;
    public int danoAtaque3 = 2;

    [Header("Debug")]
    public bool dibujarGizmos = true;

    // Internos
    private float lastAttackTime = -999f;
    private bool atacando = false;
    private float baseScaleX;
    private Vector2 moveDir;

    // Invulnerabilidad
    public float invulnerableTime = 0.5f;
    private bool invulnerable = false;

    // Control interno de vida
    private bool vivo = true; // bloquea comportamiento cuando muere

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        vida = Mathf.Clamp(vida, 0, vidaMaxima);
        baseScaleX = Mathf.Abs(transform.localScale.x);
        EncontrarJugadorYIgnorarEmpuje();
    }

    void Update()
    {
        if (!vivo) return; // bloquea todo comportamiento si está muriendo

        if (player == null)
        {
            EncontrarJugadorYIgnorarEmpuje();
            return;
        }

        Vector2 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;

        // Mirar hacia el jugador
        if (toPlayer.x < -0.01f) transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        else if (toPlayer.x > 0.01f) transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);

        moveDir = Vector2.zero;

        // Solo moverse si no está atacando
        if (!atacando)
        {
            // ÚNICO CAMBIO: Agregado "&& dist > rangoMinimoDistancia"
            if (dist <= detectionRadius && dist > rangoMinimoDistancia && dist > stopRange)
                moveDir = toPlayer.normalized;

            if (Time.time >= lastAttackTime + intervaloAtaque)
                IntentarAtacar(dist);
        }

        animator.SetFloat("Speed", moveDir == Vector2.zero ? 0f : 1f);
    }

    void FixedUpdate()
    {
        if (!vivo || atacando) return; // bloquea movimiento si está muriendo

        if (moveDir != Vector2.zero)
        {
            Vector2 destino = rb.position + moveDir * speed * Time.fixedDeltaTime;
            rb.MovePosition(destino);
        }
    }

    bool EnFase2() => vida <= (vidaMaxima / 2);

    void IntentarAtacar(float distAlJugador)
    {
        if (player == null || !vivo) return;

        if (!EnFase2())
        {
            if (distAlJugador <= rangoAtaque1)
                StartCoroutine(EjecutarAtaque(1));
            return;
        }

        if (EnFase2())
        {
            if (distAlJugador <= rangoAtaque2)
            {
                int cual = (Random.value < 0.5f) ? 2 : 3;
                if (cual == 3 && distAlJugador > rangoAtaque3) cual = 2;
                StartCoroutine(EjecutarAtaque(cual));
            }
            else if (distAlJugador <= rangoAtaque3)
            {
                StartCoroutine(EjecutarAtaque(3));
            }
        }
    }

    IEnumerator EjecutarAtaque(int cual)
    {
        atacando = true;
        lastAttackTime = Time.time;

        switch (cual)
        {
            case 1: animator.SetTrigger("Attack1"); break;
            case 2: animator.SetTrigger("Attack2"); break;
            case 3: animator.SetTrigger("Attack3"); break;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float duracion = state.length;
        yield return new WaitForSeconds(duracion);

        atacando = false;
    }

    public void AnimEvent_Attack1Hit() { AplicarDanoEnRango(rangoAtaque1, danoAtaque1); }
    public void AnimEvent_Attack2Hit() { AplicarDanoEnRango(rangoAtaque2, danoAtaque2); }
    public void AnimEvent_Attack3Hit() { AplicarDanoEnRango(rangoAtaque3, danoAtaque3); }
    public void AnimEvent_AttackEnd() { atacando = false; }

    void AplicarDanoEnRango(float rango, int dano)
    {
        if (player == null || !vivo) return;

        Vector2 punto = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D objetivo = Physics2D.OverlapCircle(punto, rango, playerLayer);

        if (objetivo != null)
        {
            moverPersonaje scriptPlayer = objetivo.GetComponent<moverPersonaje>();
            if (scriptPlayer != null && !scriptPlayer.muerto)
            {
                Vector2 dir = (objetivo.transform.position - transform.position).normalized;
                scriptPlayer.RecibeDanio(dir, dano);
            }
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantidad)
    {
        if (!vivo || invulnerable) return;

        vida -= cantidad;
        if (vida <= 0)
        {
            vida = 0;
            vivo = false; // bloquea comportamiento
            Morir();
        }
        else
        {
            StartCoroutine(ParpadeoInvulnerabilidad());
        }
    }

    IEnumerator ParpadeoInvulnerabilidad()
    {
        invulnerable = true;

        float elapsed = 0f;
        while (elapsed < invulnerableTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        spriteRenderer.enabled = true;
        invulnerable = false;
    }

    void Morir()
    {
        atacando = false;
        moveDir = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        if (bossCollider) bossCollider.enabled = false;

        animator.SetTrigger("Dead"); // dispara animación de muerte
        Destroy(gameObject, 2.0f);
    }

    void EncontrarJugadorYIgnorarEmpuje()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            playerCollider = go.GetComponent<Collider2D>();
            if (bossCollider != null && playerCollider != null)
                Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!dibujarGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // NUEVO: Visualizar distancia mínima
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoMinimoDistancia);

        Vector3 p = attackPoint ? attackPoint.position : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(p, rangoAtaque1);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(p, rangoAtaque2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(p, rangoAtaque3);
    }
}