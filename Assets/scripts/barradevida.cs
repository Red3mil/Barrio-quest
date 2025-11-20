using UnityEngine;
using UnityEngine.UI;


public class barradevida : MonoBehaviour
{
    public Slider sliderVida;
    public moverPersonaje jugador;
    private Image fillImage;

    void Start()
    {
        if (jugador != null && sliderVida != null)
        {
            sliderVida.maxValue = jugador.vidaMaxima;
            sliderVida.value = jugador.vida;

            fillImage = sliderVida.fillRect.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (jugador != null && sliderVida != null)
        {
            sliderVida.value = jugador.vida;

            float porcentaje = (float)jugador.vida / jugador.vidaMaxima;

            if (porcentaje <= 0.25f)
            {
                fillImage.color = Color.red;
            }
            else if (porcentaje <= 0.5f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.green;
            }
        }
    }
}