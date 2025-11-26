using UnityEngine;
using UnityEngine.UI;
using EasyTransition;

public class CambioDeEscenaBoton : MonoBehaviour
{
    public string nombreEscena;
    public TransitionSettings transition;
    public AudioSource sonidoClick;

    public void CargarEscena()
    {
        if (sonidoClick != null)
            sonidoClick.Play();

        TransitionManager.Instance().Transition(
            nombreEscena,
            transition,
            0f
        );
    }
}
