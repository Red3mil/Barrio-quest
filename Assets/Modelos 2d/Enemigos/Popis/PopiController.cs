using UnityEngine;
using System.Collections;

public class PopiController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int vida = 3;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.5f;
    public float rangoAtaque = 1.5f;

    [Header("Animación")]
    public string nombreAnimacionMuerte = "PopiBlancoDeath"; // Configurable desde el Inspector

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

    private bool atacando;

    public float fuerzaRebote = 5f;

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

        // Asegurarse de que el enemigo esté en la layer "Enemy"
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider2D enemigoCollider = GetComponent<Collider2D>();
        Collider2D jugadorCollider = player != null ? player.GetComponent<Collider2D>() : null;
        if (enemigoCollider != null && jugadorCollider != null)
            Physics2D.IgnoreCollision(enemigoCollider, jugadorCollider, true);

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // No actualizar nada si está muerto
        if (muerto) return;

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

        // Solo actualizar animación de movimiento si no está recibiendo daño
        if (!recibiendoDanio)
        {
            animator.SetBool("enMovimiento", enMovimiento);
        }
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius && playerVivo)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Mirar al jugador
            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            else if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

            movement = new Vector2(direction.x, direction.y);
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
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

        if (distancia <= rangoAtaque && Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques && playerVivo && !recibiendoDanio)
        {
            animator.SetTrigger("atacar");
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

            // Forzar a idle (enMovimiento = false)
            enMovimiento = false;
            animator.SetBool("enMovimiento", false);

            if (vida <= 0)
            {
                Morir();
            }
            else
            {
                // Rebote controlado
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

        // Detener movimiento
        rb.linearVelocity = Vector2.zero;

        // Deshabilitar el collider para que no interfiera
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Desactivar el script para que no procese más Update
        this.enabled = false;

        // Reproducir animación de muerte UNA SOLA VEZ y luego desactivar animator
        StartCoroutine(ReproducirMuerteYDesactivar());
    }

    IEnumerator ReproducirMuerteYDesactivar()
    {
        // Forzar la animación de muerte usando el nombre configurable
        animator.Play(nombreAnimacionMuerte, 0, 0f);

        // Esperar a que termine la animación
        yield return new WaitForSeconds(GetAnimationLength(nombreAnimacionMuerte));

        // Desactivar el animator para que no haga loop
        animator.enabled = false;

        // Esperar un poco más antes de hacer fadeout
        yield return new WaitForSeconds(0.5f);

        // Fadeout y destruir
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

    IEnumerator EliminarDespuesDeSegundos(float segundos)
    {
        // Este método ya no se usa, pero lo dejamos por compatibilidad
        yield return new WaitForSeconds(segundos);
        yield return StartCoroutine(FadeOut());
        Destroy(gameObject);
    }

    // Función auxiliar para obtener la duración de una animación
    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] clips = ac.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"No se encontró la animación '{animationName}'. Usando duración por defecto.");
        return 1f; // Valor por defecto si no encuentra la animación
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}