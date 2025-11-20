using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int vida = 3;
    public float detectionRadius = 5f;
    public float speed = 2f;
    public float fuerzaRebote = 6f;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.5f;

    private Transform player;
    private moverPersonaje playerScript;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 escalaOriginal;

    private Vector2 movimiento;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private GameObject jugadorEnRango;
    private float ultimoAtaqueTiempo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerScript = player.GetComponent<moverPersonaje>();

        ultimoAtaqueTiempo = -tiempoEntreAtaques;
    }

    void Update()
    {
        if (muerto || player == null || playerScript == null || playerScript.muerto)
        {
            enMovimiento = false;
        }
        else
        {
            Movimiento();
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }

    private void Movimiento()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= detectionRadius && !recibiendoDanio)
        {
            Vector2 direccion = (player.position - transform.position).normalized;

            // Girar sprite según la dirección en X
            if (direccion.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            else if (direccion.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

            movimiento = direccion;
            enMovimiento = true;

            rb.MovePosition(rb.position + movimiento * speed * Time.deltaTime);
        }
        else
        {
            movimiento = Vector2.zero;
            enMovimiento = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (muerto) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorEnRango = collision.gameObject;

            if (playerScript != null && !playerScript.muerto && Time.time >= ultimoAtaqueTiempo + tiempoEntreAtaques)
            {
                animator.SetTrigger("atacar");
                ultimoAtaqueTiempo = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (muerto) return;

        if (collision.CompareTag("Golpe"))
        {
            Vector2 direccionDanio = new Vector2(collision.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || muerto) return;

        vida -= cantDanio;
        recibiendoDanio = true;
        animator.SetTrigger("recibirDanio");

        if (vida <= 0)
        {
            Muerte();
        }
        else
        {
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            StartCoroutine(DesactivaDanio());
        }
    }

    private void Muerte()
    {
        muerto = true;
        enMovimiento = false;
        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("muerte");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(EliminarDespuesDeSegundos(2f));
    }

    // Método llamado desde la animación "atacar" para aplicar daño
    public void AplicarDanio()
    {
        if (muerto || jugadorEnRango == null) return;

        moverPersonaje jugador = jugadorEnRango.GetComponent<moverPersonaje>();
        if (jugador != null && !jugador.muerto && !jugador.recibiendoDanio)
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            jugador.RecibeDanio(direccionDanio, 1);
            ultimoAtaqueTiempo = Time.time;
        }
    }

    private IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator EliminarDespuesDeSegundos(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
