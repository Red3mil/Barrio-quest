using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using EasyTransition; // ? IMPORTANTE

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image panelOscuro;
    public TextMeshProUGUI textoTutorial;
    public Canvas canvasTutorial;

    [Header("Configuración Texto")]
    public float duracionFadeIn = 0.5f;
    public float tiempoMostrarTexto = 3f;
    public float duracionFadeOut = 0.5f;
    public Color colorOscuro = new Color(0, 0, 0, 0.7f);

    [Header("Transición")]
    public TransitionSettings transicion;  // ? ARCHIVO .asset AQUI

    private moverPersonaje jugador;
    private CamaraDescensoSuave camaraDescenso;
    private bool tutorialActivo = false;

    void Start()
    {
        jugador = FindFirstObjectByType<moverPersonaje>();
        camaraDescenso = Camera.main.GetComponent<CamaraDescensoSuave>();

        if (panelOscuro != null)
        {
            panelOscuro.color = new Color(0, 0, 0, 0);
            panelOscuro.gameObject.SetActive(false);
        }

        if (textoTutorial != null)
        {
            textoTutorial.gameObject.SetActive(false);
        }

        if (canvasTutorial != null)
        {
            canvasTutorial.sortingOrder = 100;
        }
    }

    public void MostrarDialogo(string texto)
    {
        if (tutorialActivo) return;
        StartCoroutine(SecuenciaDialogo(texto));
    }

    private IEnumerator SecuenciaDialogo(string texto)
    {
        tutorialActivo = true;

        // ASEGURA que timeScale esté en 1 antes de pausar
        Time.timeScale = 1f;
        yield return null; // Espera un frame

        Time.timeScale = 0f;

        if (jugador != null)
            jugador.enabled = false;
        if (panelOscuro != null) panelOscuro.gameObject.SetActive(true);
        if (textoTutorial != null) textoTutorial.gameObject.SetActive(true);
        textoTutorial.text = texto;

        // FADE IN
        float tiempo = 0;
        while (tiempo < duracionFadeIn)
        {
            tiempo += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, colorOscuro.a, tiempo / duracionFadeIn);
            panelOscuro.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(tiempoMostrarTexto);

        // FADE OUT
        tiempo = 0;
        while (tiempo < duracionFadeOut)
        {
            tiempo += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(colorOscuro.a, 0, tiempo / duracionFadeOut);
            panelOscuro.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (panelOscuro != null) panelOscuro.gameObject.SetActive(false);
        if (textoTutorial != null) textoTutorial.gameObject.SetActive(false);

        // RESTAURA SIEMPRE
        Time.timeScale = 1f;

        if (jugador != null)
            jugador.enabled = true;

        tutorialActivo = false;
    }

    public void BloquearCamara(Vector3 posicion)
    {
        if (camaraDescenso != null)
            camaraDescenso.BloquearEnPosicion(posicion);
    }

    public void DesbloquearCamara()
    {
        if (camaraDescenso != null)
            camaraDescenso.SeguirJugador();
    }

    // ?? NUEVO MÉTODO DE TRANSICIÓN
    public void TransicionLentaCambioEscena(string nombreEscena, float delay = 0f)
    {
        // FUERZA timeScale a 1 antes de cambiar escena
        Time.timeScale = 1f;

        // Asegura que el jugador esté habilitado
        if (jugador != null)
            jugador.enabled = true;

        TransitionManager.Instance().Transition(
            nombreEscena,
            transicion,
            delay
        );
    }

    public bool EsTutorialActivo()
    {
        return tutorialActivo;
    }
}
