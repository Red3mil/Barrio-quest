using UnityEngine;
using System.Collections;

public enum TipoTutorial
{
    Movimiento,
    Dash,
    Atacar,
    Curacion
}

public class ZonaTutorial : MonoBehaviour
{
    [Header("Configuración")]
    public TipoTutorial tipoTutorial;
    public string textoTutorial = "Presiona las flechas para moverte";

    [Header("Objetos a activar")]
    public GameObject npcMentor;
    public GameObject munecoAtaque;
    public GameObject[] curaciones;

    [Header("Referencias")]
    public TutorialManager tutorialManager;

    [Header("Configuración de Muros")]
    public float anchoMuro = 1f;
    public float alturaMuro = 20f;

    [Header("Configuración de Desaparición")]
    public bool desaparecerConEfecto = true;
    public float tiempoEfectoDesaparicion = 0.5f;

    [Header("Configuración Tutorial Curación")]
    public int danoParaCuracion = 5;

    private bool tutorialCompletado = false;
    private bool tutorialIniciado = false;
    private moverPersonaje jugador;
    private CamaraDescensoSuave camaraDescenso;

    private GameObject muroIzquierdo;
    private GameObject muroDerecho;

    private bool seMovio = false;
    private bool hizoAtaque = false;
    private bool hizoDash = false;
    private int vidaInicial = 0;

    void Start()
    {
        if (npcMentor != null) npcMentor.SetActive(false);
        if (munecoAtaque != null) munecoAtaque.SetActive(false);

        foreach (GameObject curacion in curaciones)
        {
            if (curacion != null) curacion.SetActive(false);
        }

        camaraDescenso = Camera.main.GetComponent<CamaraDescensoSuave>();
        jugador = FindFirstObjectByType<moverPersonaje>();
    }

    void Update()
    {
        if (!tutorialIniciado || tutorialCompletado) return;

        switch (tipoTutorial)
        {
            case TipoTutorial.Movimiento:
                VerificarMovimiento();
                break;
            case TipoTutorial.Dash:
                VerificarDash();
                break;
            case TipoTutorial.Atacar:
                VerificarAtaque();
                break;
            case TipoTutorial.Curacion:
                VerificarCuracion();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tutorialCompletado || tutorialIniciado) return;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(EsperarTransicionAntesDeTutorial());
        }
    }

    private void IniciarTutorial()
    {
        tutorialIniciado = true;

        if (npcMentor != null) npcMentor.SetActive(true);
        if (munecoAtaque != null && tipoTutorial == TipoTutorial.Atacar)
            munecoAtaque.SetActive(true);

        if (tipoTutorial == TipoTutorial.Curacion)
        {
            foreach (GameObject curacion in curaciones)
                if (curacion != null) curacion.SetActive(true);

            if (jugador != null && jugador.vida > danoParaCuracion)
            {
                jugador.vida -= danoParaCuracion;
                vidaInicial = jugador.vida;
            }
            else if (jugador != null)
            {
                vidaInicial = jugador.vida;
            }
        }

        if (camaraDescenso != null)
            camaraDescenso.BloquearEnPosicion(Camera.main.transform.position);

        CrearMuros();

        if (tutorialManager != null)
            tutorialManager.MostrarDialogo(textoTutorial);
    }

    private void CrearMuros()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float alturaCamera = 2f * cam.orthographicSize;
        float anchoCamera = alturaCamera * cam.aspect;

        Vector3 posicionCamara = cam.transform.position;

        muroIzquierdo = new GameObject("MuroIzquierdo_Tutorial");
        BoxCollider2D colliderIzq = muroIzquierdo.AddComponent<BoxCollider2D>();
        colliderIzq.size = new Vector2(anchoMuro, alturaMuro);

        float posX = posicionCamara.x - (anchoCamera / 2f) - (anchoMuro / 2f);
        muroIzquierdo.transform.position = new Vector3(posX, posicionCamara.y, 0);

        muroDerecho = new GameObject("MuroDerecho_Tutorial");
        BoxCollider2D colliderDer = muroDerecho.AddComponent<BoxCollider2D>();
        colliderDer.size = new Vector2(anchoMuro, alturaMuro);

        posX = posicionCamara.x + (anchoCamera / 2f) + (anchoMuro / 2f);
        muroDerecho.transform.position = new Vector3(posX, posicionCamara.y, 0);
    }

    private void VerificarMovimiento()
    {
        if (jugador == null) return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            seMovio = true;

        if (seMovio)
            CompletarTutorial();
    }

    private void VerificarDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            hizoDash = true;

        if (hizoDash)
            CompletarTutorial();
    }

    private void VerificarAtaque()
    {
        if (Input.GetKeyDown(KeyCode.C))
            hizoAtaque = true;

        if (hizoAtaque)
            CompletarTutorial();
    }

    private void VerificarCuracion()
    {
        if (jugador == null) return;

        if (jugador.vida > vidaInicial)
            CompletarTutorial();
    }

    private void CompletarTutorial()
    {
        tutorialCompletado = true;

        if (jugador != null)
        {
            Animator animatorJugador = jugador.GetComponent<Animator>();
            if (animatorJugador != null)
                animatorJugador.speed = 1;
        }

        if (muroIzquierdo != null) Destroy(muroIzquierdo);
        if (muroDerecho != null) Destroy(muroDerecho);

        if (camaraDescenso != null)
            camaraDescenso.SeguirJugador();

        if (desaparecerConEfecto)
            StartCoroutine(DesaparecerObjetosConEfecto());
        else
            DestruirObjetosInstantaneo();

        Debug.Log("Tutorial " + tipoTutorial + " completado!");

        // ?? AQUI ES DONDE SE CAMBIA LA ESCENA CON TRANSICIÓN
        if (tipoTutorial == TipoTutorial.Curacion)
        {
            tutorialManager.TransicionLentaCambioEscena("SampleScene", 0f);
        }
    }

    private void DestruirObjetosInstantaneo()
    {
        if (npcMentor != null) Destroy(npcMentor);
        if (munecoAtaque != null) Destroy(munecoAtaque);

        foreach (GameObject curacion in curaciones)
            if (curacion != null) Destroy(curacion);
    }

    private IEnumerator EsperarTransicionAntesDeTutorial()
    {
        // Solo ejecutar si el tipo de tutorial es Movimiento
        if (tipoTutorial == TipoTutorial.Movimiento)
        {
            // Espera 1.25 segundos para dejar que la transición IN termine
            yield return new WaitForSeconds(1.25f);

            IniciarTutorial();
        }
        else
        {
            // Si no es Movimiento, iniciar tutorial inmediatamente
            IniciarTutorial();
            yield break;
        }
    }

    private IEnumerator DesaparecerObjetosConEfecto()
    {
        GameObject[] objetosADesaparecer = new GameObject[1 + (munecoAtaque != null ? 1 : 0) + curaciones.Length];
        int index = 0;

        if (npcMentor != null) objetosADesaparecer[index++] = npcMentor;
        if (munecoAtaque != null) objetosADesaparecer[index++] = munecoAtaque;

        foreach (GameObject curacion in curaciones)
            if (curacion != null) objetosADesaparecer[index++] = curacion;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoEfectoDesaparicion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / tiempoEfectoDesaparicion);

            foreach (GameObject obj in objetosADesaparecer)
            {
                if (obj != null)
                {
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color color = sr.color;
                        color.a = alpha;
                        sr.color = color;
                    }
                }
            }

            yield return null;
        }

        foreach (GameObject obj in objetosADesaparecer)
            if (obj != null) Destroy(obj);
    }

    void OnDestroy()
    {
        if (muroIzquierdo != null) Destroy(muroIzquierdo);
        if (muroDerecho != null) Destroy(muroDerecho);
    }
}
