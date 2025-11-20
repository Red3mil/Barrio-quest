using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class zonadeCombate : MonoBehaviour
{
    [Header("Config Stage")]
    public int numeroStage = 1;
    public int totalStages = 3;

    [Header("Spawn Enemigos")]
    public Transform[] puntosSpawn;
    public GameObject[] tiposEnemigos;
    public int cantidadEnemigos = 5;
    public float delaySpawn = 0.5f;

    [Header("Referencias")]
    public CamaraDescensoSuave camaraScript;
    public moverPersonaje jugador;
    public TextMeshProUGUI textoStageUI;
    public TextMeshProUGUI textoGoUI;

    private bool combateIniciado = false;
    private List<GameObject> enemigosEnEscena = new List<GameObject>();

    private float limiteMinX, limiteMaxX;

    private void Awake()
    {
        textoStageUI.gameObject.SetActive(false);
        textoGoUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!combateIniciado && collision.CompareTag("Player"))
        {
            combateIniciado = true;

            Vector3 centroZona = transform.position;
            camaraScript.BloquearEnPosicion(centroZona);

            Camera cam = camaraScript.GetCamara();
            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            limiteMinX = centroZona.x - camWidth / 2.2f;
            limiteMaxX = centroZona.x + camWidth / 2.2f;

            jugador.limiteIzquierdo = limiteMinX;
            jugador.limiteDerecho = limiteMaxX;

            StartCoroutine(MostrarStageYEsperarSpawn());
        }
    }

    IEnumerator MostrarStageYEsperarSpawn()
    {
        textoStageUI.gameObject.SetActive(true);
        textoStageUI.text = $"Stage {numeroStage}/{totalStages}";

        yield return StartCoroutine(FadeText(textoStageUI, true, 1f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeText(textoStageUI, false, 1f));

        textoStageUI.gameObject.SetActive(false);

        yield return StartCoroutine(SpawnEnemigos());

        InvokeRepeating(nameof(VerificarEnemigos), 1f, 1f);
    }

    IEnumerator SpawnEnemigos()
    {
        for (int i = 0; i < cantidadEnemigos; i++)
        {
            Transform spawnPoint = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
            GameObject enemigoPrefab = tiposEnemigos[Random.Range(0, tiposEnemigos.Length)];
            GameObject enemigo = Instantiate(enemigoPrefab, spawnPoint.position, Quaternion.identity);
            enemigosEnEscena.Add(enemigo);
            yield return new WaitForSeconds(delaySpawn);
        }
    }

    void VerificarEnemigos()
    {
        enemigosEnEscena.RemoveAll(e => e == null);

        if (enemigosEnEscena.Count == 0)
        {
            TerminarCombate();
        }
    }

    void TerminarCombate()
    {
        CancelInvoke(nameof(VerificarEnemigos));

        camaraScript.SeguirJugador();

        jugador.limiteIzquierdo = -1000f;
        jugador.limiteDerecho = 1000f;

        StartCoroutine(MostrarGO());

        Destroy(gameObject, 1f);
    }

    IEnumerator MostrarGO()
    {
        textoGoUI.gameObject.SetActive(true);
        textoGoUI.text = "GO!";

        float duracion = 1f;
        float tiempo = 0f;

        Color color = textoGoUI.color;

        while (tiempo < duracion)
        {
            // Escala con sin para agrandar y achicar
            float escala = 1f + Mathf.Sin(tiempo * 10f) * 0.2f;
            textoGoUI.transform.localScale = Vector3.one * escala;

            // Fade out alpha desde 1 hasta 0
            color.a = Mathf.Lerp(1f, 0f, tiempo / duracion);
            textoGoUI.color = color;

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Reset scale y alpha al final para evitar interferencias
        textoGoUI.transform.localScale = Vector3.one;
        color.a = 0f;
        textoGoUI.color = color;
        textoGoUI.gameObject.SetActive(false);
    }

    IEnumerator FadeText(TextMeshProUGUI tmp, bool fadeIn, float duration)
    {
        float tiempo = 0f;
        Color color = tmp.color;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (tiempo < duration)
        {
            tiempo += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, tiempo / duration);
            tmp.color = color;
            yield return null;
        }
    }
}