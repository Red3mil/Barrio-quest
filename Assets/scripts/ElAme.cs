using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ElAme : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public float fuerzaRebote = 6f;
    public int vida = 3;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;
    private GameObject jugadorEnRango;
    private Animator animator;
    private Vector3 escalaOriginal;
    private moverPersonaje playerScript;
    private float tiempoEntreAtaques = 1.5f;
    private float ultimoAtaqueTiempo = -999f;

    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;

        if (player != null)
        {
            playerScript = player.GetComponent<moverPersonaje>();
        }
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
            Movimiento();
        }
        else
        {
            enMovimiento = false;
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius && playerVivo && !muerto)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            else if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;

            if (!recibiendoDanio)
            {
                rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
            }
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            moverPersonaje playerScriptCollision = collision.gameObject.GetComponent<moverPersonaje>();

            if (playerScriptCollision != null && !playerScriptCollision.muerto)
            {
                animator.SetTrigger("atacar");

                Vector2 direccionDanio = new Vector2(transform.position.x, 0);
                playerScriptCollision.RecibeDanio(direccionDanio, 1);

                playerVivo = !playerScriptCollision.muerto;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            moverPersonaje playerScript = collision.gameObject.GetComponent<moverPersonaje>();
            jugadorEnRango = collision.gameObject;

            if (playerScript != null && !playerScript.muerto && Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques)
            {
                int ataque = Random.Range(0, 3); // 0, 1, 2
                animator.SetInteger("tipoAtaque", ataque);
                animator.SetTrigger("atacar");

                ultimoAtaqueTiempo = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Golpe"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            animator.SetTrigger("recibirDanio");
            RecibeDanio(direccionDanio, 1);
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
                StartCoroutine(Blink());
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }
        }
    }

    public void AplicarDanio()
    {
        if (jugadorEnRango != null && Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques)
        {
            moverPersonaje playerScript = jugadorEnRango.GetComponent<moverPersonaje>();

            if (playerScript != null && !playerScript.muerto && !playerScript.recibiendoDanio)
            {
                Vector2 direccionDanio = new Vector2(transform.position.x, 0);
                playerScript.RecibeDanio(direccionDanio, 2); // 🔸 2 es el daño del jefe
                ultimoAtaqueTiempo = Time.time;
            }
        }
    }

    IEnumerator DesactivaDanio()
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
        EliminarCuerpo();
    }

    public void EliminarCuerpo()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}