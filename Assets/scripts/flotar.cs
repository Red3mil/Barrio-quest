using UnityEngine;

public class flotar : MonoBehaviour
{
    [Header("Movimiento de flotación")]
    public float amplitud = 0.5f;
    public float velocidad = 2f;

    [Header("Fade In")]
    public float duracionFade = 1f;

    private Vector3 posicionInicial;
    private Vector3 offset;
    private bool arrastrando = false;
    private float tiempoInicio;

    private SpriteRenderer sr;

    void Start()
    {
        posicionInicial = transform.position;
        tiempoInicio = Time.time;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f; // Comenzar invisible
            sr.color = c;
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (!arrastrando)
        {
            float nuevaY = Mathf.Sin((Time.time - tiempoInicio) * velocidad) * amplitud;
            transform.position = new Vector3(posicionInicial.x, posicionInicial.y + nuevaY, posicionInicial.z);
        }
    }

    void OnMouseDown()
    {
        arrastrando = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }

    void OnMouseDrag()
    {
        if (arrastrando)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z) + offset;
        }
    }

    void OnMouseUp()
    {
        arrastrando = false;
        transform.position = posicionInicial;
        tiempoInicio = Time.time; // Reiniciar flotación
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duracionFade);
            if (sr != null)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            yield return null;
        }
    }
}