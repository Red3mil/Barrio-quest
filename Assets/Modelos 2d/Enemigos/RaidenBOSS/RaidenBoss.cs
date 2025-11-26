using UnityEngine;
using System.Collections;

public class RaidenBoss : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 1.5f;
    public int vida = 20;

    [Header("Rangos de Ataque")]
    public float rangoAtaqueCuerpoACuerpo = 2.0f;
    public float rangoMinimoDistancia = 1.5f;
    public float rangoMinimoDisparo = 3.0f;
    public float rangoMaximoDisparo = 8.0f;
    public float tiempoEntreAtaques = 2.0f;

    [Header("Proyectil (para Shoot)")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float velocidadProyectil = 8f;

    [Header("Audio")]
    public AudioClip sonidoDisparo;
    private AudioSource audioSource;

    [Header("Animación")]
    public string nombreAnimacionMuerte = "IdleDeath";

    private Rigidbody2D rb;
    private Animator animator;
    private moverPersonaje playerScript;

    private Vector3 escalaOriginal;

    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo = true;
    private bool atacando;
    private float ultimoAtaqueTiempo = -999f;

    private float duracionAtaqueMax = 1.2f;
    private float tiempoAtaqueActual = 0;

    public float fuerzaRebote = 3f;

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider2D enemigoCollider = GetComponent<Collider2D>();
        Collider2D jugadorCollider = player != null ? player.GetComponent<Collider2D>() : null;

        if (enemigoCollider != null && jugadorCollider != null)
            Physics2D.IgnoreCollision(enemigoCollider, jugadorCollider, true);

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (muerto) return;

        // ?? TERMINAR ATAQUE POR TIMER DE SEGURIDAD
        if (atacando)
        {
            tiempoAtaqueActual -= Time.deltaTime;
            if (tiempoAtaqueActual <= 0)
                TerminarAtaque();
        }

        if (playerScript != null && playerScript.vida <= 0)
        {
            enMovimiento = false;
            playerVivo = false;
        }
        else if (playerVivo)
        {
            if (!atacando && !recibiendoDanio)
                Movimiento();

            RevisarAtaquePorDistancia();
        }
        else
        {
            enMovimiento = false;
        }

        if (!recibiendoDanio)
            animator.SetBool("enMovimiento", enMovimiento);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius &&
            distanceToPlayer > rangoMinimoDistancia &&
            playerVivo &&
            !atacando)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Mirarlo
            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            enMovimiento = true;
        }
        else
        {
            enMovimiento = false;
        }
    }

    private void RevisarAtaquePorDistancia()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if (Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques &&
            playerVivo && !recibiendoDanio && !atacando)
        {
            // Cuerpo a cuerpo
            if (distancia <= rangoAtaqueCuerpoACuerpo)
            {
                int ataque = Random.Range(0, 2);

                if (ataque == 0)
                    animator.SetTrigger("punch");
                else
                    animator.SetTrigger("swordStab");

                IniciarAtaque();
            }
            // Disparo
            else if (distancia >= rangoMinimoDisparo && distancia <= rangoMaximoDisparo)
            {
                animator.SetTrigger("shoot");
                IniciarAtaque();
            }
        }
    }

    private void IniciarAtaque()
    {
        atacando = true;
        tiempoAtaqueActual = duracionAtaqueMax; // ?? Timer de seguridad
        ultimoAtaqueTiempo = Time.time;
        animator.SetBool("enMovimiento", false);
    }

    // ?? Evento que termina el ataque (Animation Event o Timer)
    public void TerminarAtaque()
    {
        atacando = false;
        tiempoAtaqueActual = 0;

        animator.ResetTrigger("punch");
        animator.ResetTrigger("swordStab");
        animator.ResetTrigger("shoot");
    }

    // ?? Evento de daño Punch
    public void AplicarDanioPunch()
    {
        if (muerto) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= rangoAtaqueCuerpoACuerpo && playerScript != null &&
            !playerScript.muerto && !playerScript.recibiendoDanio)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            playerScript.RecibeDanio(dir, 1);
        }
    }

    // ?? Evento de daño SwordStab
    public void AplicarDanioSwordStab()
    {
        if (muerto) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= rangoAtaqueCuerpoACuerpo && playerScript != null &&
            !playerScript.muerto && !playerScript.recibiendoDanio)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            playerScript.RecibeDanio(dir, 2);
        }
    }

    // ?? Evento de disparo
    public void DispararProyectil()
    {
        if (muerto || proyectilPrefab == null) return;

        // ?? Reproducir sonido del disparo
        if (sonidoDisparo != null && audioSource != null)
            audioSource.PlayOneShot(sonidoDisparo);

        // Determinar punto de origen del disparo
        Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position;

        // Calcular dirección hacia el jugador
        Vector2 direccion = (player.position - origen).normalized;

        // Instanciar proyectil
        GameObject proyectil = Instantiate(proyectilPrefab, origen, Quaternion.identity);

        // Configurar velocidad del proyectil
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();
        if (rbProyectil != null)
        {
            rbProyectil.linearVelocity = direccion * velocidadProyectil;
        }

        // Rotar proyectil para que mire hacia donde va
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        proyectil.transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio && !muerto)
        {
            vida -= cantDanio;
            recibiendoDanio = true;
            enMovimiento = false;
            animator.SetBool("enMovimiento", false);

            if (vida <= 0)
            {
                Morir();
            }
            else
            {
                if (direccion != Vector2.zero)
                {
                    Vector2 rebote = -direccion.normalized * fuerzaRebote * 0.1f;
                    rb.MovePosition(rb.position + rebote);
                }

                StartCoroutine(Blink());
                StartCoroutine(ResetDanio());
            }
        }
    }

    private void Morir()
    {
        muerto = true;
        enMovimiento = false;
        atacando = false;
        recibiendoDanio = false;

        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(ReproducirMuerteYDesactivar());
    }

    IEnumerator ReproducirMuerteYDesactivar()
    {
        animator.Play(nombreAnimacionMuerte, 0, 0f);
        yield return new WaitForSeconds(GetAnimationLength(nombreAnimacionMuerte));

        animator.enabled = false;
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeOut());
        Destroy(gameObject);
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

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] clips = ac.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }

        return 1f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaqueCuerpoACuerpo);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoMinimoDisparo);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoMaximoDisparo);
    }
}
