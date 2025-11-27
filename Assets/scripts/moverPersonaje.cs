using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class moverPersonaje : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float limiteSuperior = 5f;
    public float limiteInferior = -2f;
    public float limiteIzquierdo = -1000f;
    public float limiteDerecho = 1000f;

    [Header("Vida")]
    public int vidaMaxima = 10;
    public int vida = 10;

    [Header("UI Muerte")]
    public GameObject canvasMuerte;

    [Header("Dash")]
    public float fuerzaDash = 8f;
    public float tiempoDash = 0.3f;

    [Header("Ataque")]
    public float ataqueCooldown = 0.5f;
    public AudioClip sonidoAtaque;
    public ParticleSystem particulasGolpe;

    [Header("Efectos")]
    public ParticleSystem polvoMovimiento;

    [Header("Daño")]
    public LayerMask enemyLayer;
    public float rangoAtaque = 1.5f;
    public int danoAtaque = 1;

    [Header("Cámara")]
    public CamaraShake camaraShake;
    public float duracionShake = 0.2f;
    public float intensidadShake = 0.1f;

    [Header("Pausa")]   // ⭐ NUEVO
    public GameObject canvasPausa;
    private bool juegoPausado = false;

    private float escalaOriginalX;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    public bool recibiendoDanio;
    private bool haciendoDash;
    public bool muerto;
    private bool atacando;
    private float tiempoUltimoAtaque;

    private Vector2 inputMovimiento;
    private int vidaInicial;
    private int enemyLayerNum;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        escalaOriginalX = transform.localScale.x;
        vida = Mathf.Clamp(vidaMaxima, 0, vidaMaxima);
        vidaInicial = vida;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        enemyLayerNum = LayerMask.NameToLayer("Enemy");

        if (canvasPausa != null)
            canvasPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!juegoPausado)
                PausarJuego();
            else
                ReanudarJuego();
        }

        if (juegoPausado || muerto)
            return;

        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) inputVertical = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) inputVertical = -1f;

        inputMovimiento = new Vector2(inputHorizontal, inputVertical).normalized;

        if (Input.GetKeyDown(KeyCode.C) && Time.time >= tiempoUltimoAtaque + ataqueCooldown)
        {
            Atacar();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !haciendoDash && !recibiendoDanio)
        {
            StartCoroutine(Dash());
        }

        animator.SetFloat("Speed", Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical));
        animator.SetBool("recibirDanio", recibiendoDanio);
        animator.SetBool("muerto", muerto);
        animator.SetBool("Atacando", atacando);

        if (inputHorizontal < 0)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginalX), transform.localScale.y, transform.localScale.z);
        else if (inputHorizontal > 0)
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginalX), transform.localScale.y, transform.localScale.z);

        if (inputMovimiento.magnitude > 0)
        {
            if (!polvoMovimiento.isPlaying) polvoMovimiento.Play();
        }
        else
        {
            if (polvoMovimiento.isPlaying) polvoMovimiento.Stop();
        }
    }

    void FixedUpdate()
    {
        if (juegoPausado) return;
        if (recibiendoDanio || haciendoDash) return;

        Vector2 nuevaPos = rb.position + inputMovimiento * velocidad * Time.fixedDeltaTime;
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, limiteIzquierdo, limiteDerecho);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, limiteInferior, limiteSuperior);

        rb.MovePosition(nuevaPos);
    }

    private void PausarJuego()
    {
        Time.timeScale = 0f;
        juegoPausado = true;

        if (canvasPausa != null)
            canvasPausa.SetActive(true);
    }

    private void ReanudarJuego()
    {
        Time.timeScale = 1f;
        juegoPausado = false;

        if (canvasPausa != null)
            canvasPausa.SetActive(false);
    }
    private void Atacar()
    {
        atacando = true;
        tiempoUltimoAtaque = Time.time;

        if (sonidoAtaque && audioSource)
            audioSource.PlayOneShot(sonidoAtaque);

        if (particulasGolpe)
            particulasGolpe.Play();

        if (camaraShake != null)
        {
            camaraShake.Shake(duracionShake, intensidadShake);
        }

        Collider2D[] enemigosEnRango = Physics2D.OverlapCircleAll(transform.position, rangoAtaque, enemyLayer);

        foreach (Collider2D enemigo in enemigosEnRango)
        {
            BossController enemigoScript = enemigo.GetComponent<BossController>();
            PopiController popiScript = enemigo.GetComponent<PopiController>();
            RaidenBoss raidenBoss = enemigo.GetComponent<RaidenBoss>();
            AmetBoss jefeScript = enemigo.GetComponent<AmetBoss>();
            MunecoPractica munecoScript = enemigo.GetComponent<MunecoPractica>();

            if (enemigoScript != null)
                enemigoScript.RecibeDanio(Vector2.zero, danoAtaque);

            else if (popiScript != null)
                popiScript.RecibeDanio(Vector2.zero, danoAtaque);

            else if (raidenBoss != null)
                raidenBoss.RecibeDanio(Vector2.zero, danoAtaque);

            else if (jefeScript != null)
                jefeScript.RecibeDanio(Vector2.zero, danoAtaque);

            else if (munecoScript != null)
                munecoScript.RecibeDanio(Vector2.zero, danoAtaque);
        }

        StartCoroutine(ResetAtaque());
    }

    private IEnumerator ResetAtaque()
    {
        yield return new WaitForSeconds(0.2f);
        atacando = false;
    }

    private IEnumerator Dash()
    {
        haciendoDash = true;
        animator.SetTrigger("rodar");

        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerNum, true);

        float direccion = transform.localScale.x > 0 ? 1 : -1;
        float tiempo = 0f;
        float yOriginal = rb.position.y;

        while (tiempo < tiempoDash)
        {
            Vector2 posicion = rb.position;
            posicion.x += direccion * fuerzaDash * Time.fixedDeltaTime;
            posicion.y = yOriginal;
            rb.MovePosition(posicion);

            tiempo += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerNum, false);
        haciendoDash = false;
    }

    public void Curar(int cantidad)
    {
        vida += cantidad;
        if (vida > vidaMaxima) vida = vidaMaxima;
    }

    public void RestaurarVidaOriginal()
    {
        vida = vidaInicial;
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || haciendoDash) return;

        vida -= cantDanio;

        if (vida <= 0)
        {
            muerto = true;

            // PAUSA POR MUERTE
            Time.timeScale = 0f;
            if (canvasMuerte != null)
                canvasMuerte.SetActive(true);

            return;
        }

        StartCoroutine(BloqueoCortoMovimiento());
        StartCoroutine(InvulnerabilidadVisual());
    }

    private IEnumerator BloqueoCortoMovimiento()
    {
        recibiendoDanio = true;
        yield return new WaitForSeconds(0.2f);
        recibiendoDanio = false;
    }

    private IEnumerator InvulnerabilidadVisual()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerNum, true);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float tiempo = 1f;
        float intervalo = 0.1f;
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempo)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(intervalo);
            sr.enabled = true;
            yield return new WaitForSeconds(intervalo);
            tiempoPasado += intervalo * 2;
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerNum, false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}