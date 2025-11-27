using EasyTransition;
using UnityEngine;

public class TransicionPorTiempo : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public int indiceEscena = 1;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoEspera = 3f; // Tiempo en segundos antes de cambiar

    [Header("Transición a usar")]
    public TransitionSettings transition;

    private float tiempoTranscurrido = 0f;
    private bool transicionHecha = false;

    void Update()
    {
        if (!transicionHecha)
        {
            tiempoTranscurrido += Time.deltaTime;

            if (tiempoTranscurrido >= tiempoEspera)
            {
                HacerTransicion();
            }
        }
    }

    void HacerTransicion()
    {
        transicionHecha = true;

        TransitionManager.Instance().Transition(
            indiceEscena,
            transition,
            0f
        );
    }
}