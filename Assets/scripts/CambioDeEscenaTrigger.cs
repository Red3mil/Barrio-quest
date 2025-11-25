using EasyTransition;
using UnityEngine;

public class CambioDeEscenaTrigger : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string nombreEscena;

    [Header("Transición a usar")]
    public TransitionSettings transition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Solo si el que entra es el jugador
        {
            if (!string.IsNullOrEmpty(nombreEscena))
            {
                TransitionManager.Instance().Transition(
                    nombreEscena,
                    transition,
                    0f
                );
            }
            else
            {
                Debug.LogWarning("No se ha asignado el nombre de la escena en el Inspector.");
            }
        }
    }
}
