using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaMotor : MonoBehaviour
{
    [Header("Configuración UI")]
    public Image imagenBarra;      
    public Image imagenRelleno;     

    [Header("Fuente de Vida")]
    public PersonajeEnMotor personaje;

    void Start()
    {
        if (personaje != null)
        {
            ActualizarBarra();
        }
    }

    void Update()
    {
        if (personaje == null || imagenBarra == null) return;

        ActualizarBarra();
    }

    void ActualizarBarra()
    {
        float porcentaje = (float)personaje.vidaActual / personaje.vidaMaxima;

        // Control del fill
        imagenBarra.fillAmount = porcentaje;

        // Cambiar color
        if (porcentaje <= 0.25f)
            imagenBarra.color = Color.red;
        else if (porcentaje <= 0.5f)
            imagenBarra.color = Color.red;
        else
            imagenBarra.color = Color.red;
    }
}