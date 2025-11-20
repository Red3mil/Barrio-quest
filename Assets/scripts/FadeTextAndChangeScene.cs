using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeTextAndChangeScene : MonoBehaviour
{
    [TextArea]
    public string textoQueQuieroQueDiga = "Aquí escribes el texto desde el Inspector";

    public TextMeshProUGUI texto; // Asigna el texto en el inspector
    public float duracionFade = 2f; // Tiempo del fade in/out
    public float tiempoEnPantalla = 2f; // Tiempo antes del fade out
    public string nombreEscenaDestino; // Pon aquí el nombre de la escena a cargar

    void Start()
    {
        if (texto != null)
        {
            texto.text = textoQueQuieroQueDiga;
            StartCoroutine(FadeInOut());
        }
    }

    IEnumerator FadeInOut()
    {
        // Fade In
        yield return StartCoroutine(FadeTexto(0f, 1f));

        // Esperar con texto visible
        yield return new WaitForSeconds(tiempoEnPantalla);

        // Fade Out
        yield return StartCoroutine(FadeTexto(1f, 0f));

        // Cambiar de escena
        SceneManager.LoadScene(nombreEscenaDestino);
    }

    IEnumerator FadeTexto(float inicio, float fin)
    {
        float tiempo = 0f;
        Color color = texto.color;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(inicio, fin, tiempo / duracionFade);
            texto.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Asegurarse que quede en el valor final exacto
        texto.color = new Color(color.r, color.g, color.b, fin);
    }
}