using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PersonajeEnMotor : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public float velocidad = 5f;
    public float limiteSuperior = 5f;
    public float limiteInferior = -2f;

     [Header("Pausa")]  
    public GameObject canvasPausa;
    private bool juegoPausado = false;

    [Header("Vida")]
    public int vidaMaxima = 10;
    public int vidaActual = 10;

    [Header("UI Muerte")]
    public GameObject canvasMuerte; // NUEVO

    [Header("Partículas Constantes")]
    public ParticleSystem particulasConstantes;

    [Header("Invencibilidad")]
    public float duracionInvencible = 1.5f;
    public float velocidadParpadeo = 0.1f;
    private bool esInvencible = false;

    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        if (particulasConstantes != null && !particulasConstantes.isPlaying)
            particulasConstantes.Play();

        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
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
        // ------------------------------------------------

        // Si está pausado, no permitir control del jugador
        if (juegoPausado)
            return;
        float inputVertical = Input.GetAxisRaw("Vertical");

        Vector2 nuevaPos = rb.position + new Vector2(0, inputVertical * velocidad * Time.deltaTime);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, limiteInferior, limiteSuperior);

        rb.MovePosition(nuevaPos);
    }

    public void RecibirDanio(int cantidad)
    {
        if (esInvencible) return;

        vidaActual -= cantidad;

        StartCoroutine(InvencibilidadCorutina());

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    public void Curarse(int cantidad)
    {
        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMaxima);
    }

    private void Morir()
    {
        // PAUSAR JUEGO
        Time.timeScale = 0f;

        // MOSTRAR CANVAS DE MUERTE
        if (canvasMuerte != null)
            canvasMuerte.SetActive(true);

        // Quitado:
        // Destroy(gameObject);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable()
    {
        if (particulasConstantes != null)
            particulasConstantes.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstaculo"))
        {
            RecibirDanio(1);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Curar"))
        {
            Curarse(3);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator InvencibilidadCorutina()
    {
        esInvencible = true;
        float tiempo = 0f;

        while (tiempo < duracionInvencible)
        {
            CambiarAlpha(0f);
            yield return new WaitForSeconds(velocidadParpadeo);

            CambiarAlpha(1f);
            yield return new WaitForSeconds(velocidadParpadeo);

            tiempo += velocidadParpadeo * 2;
        }

        CambiarAlpha(1f);
        esInvencible = false;
    }

    private void CambiarAlpha(float alpha)
    {
        foreach (var sr in sprites)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }
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
}