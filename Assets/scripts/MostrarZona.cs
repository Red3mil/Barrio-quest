using UnityEngine;
using TMPro;
using System.Collections;

public class MostrarZona : MonoBehaviour
{
    public TextMeshProUGUI textoZona;
    public string nombreZona = "Barrio La Ciénaga";

    public float duracionFadeIn = 1f;
    public float tiempoVisible = 2f;
    public float duracionFadeOut = 1f;

    void Start()
    {
        StartCoroutine(MostrarNombreZona());
    }

    IEnumerator MostrarNombreZona()
    {
        // Activa el texto y configura el alpha en 0 (transparente)
        textoZona.gameObject.SetActive(true);
        textoZona.text = nombreZona;

        Color color = textoZona.color;
        color.a = 0;
        textoZona.color = color;

        // FADE IN
        float t = 0;
        while (t < duracionFadeIn)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(t / duracionFadeIn);
            textoZona.color = color;
            yield return null;
        }

        // ESPERA VISIBLE
        yield return new WaitForSeconds(tiempoVisible);

        // FADE OUT
        t = 0;
        while (t < duracionFadeOut)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (t / duracionFadeOut));
            textoZona.color = color;
            yield return null;
        }

        // Desactiva el texto
        textoZona.gameObject.SetActive(false);
    }
}