using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class textoparpadeante : MonoBehaviour
{

    public Text texto;
    public float duracionFadeIn = 1f;
    public float velocidadParpadeo = 1f;

    private float alpha = 0f;
    private bool fadeInCompleto = false;

    void Start()
    {
        SetAlpha(0f);
    }

    void Update()
    {
        if (!fadeInCompleto)
        {
            alpha += Time.deltaTime / duracionFadeIn;
            SetAlpha(alpha);

            if (alpha >= 1f)
            {
                alpha = 1f;
                fadeInCompleto = true;
            }
        }
        else
        {
            float alphaOscilado = 0.3f + 0.7f * Mathf.Abs(Mathf.Sin(Time.time * velocidadParpadeo));
            SetAlpha(alphaOscilado);
        }
    }

    void SetAlpha(float valor)
    {
        Color c = texto.color;
        c.a = valor;
        texto.color = c;
    }
}