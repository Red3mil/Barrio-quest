using EasyTransition;
using UnityEngine;

public class CargarEscenaPorIndice : MonoBehaviour
{
    public int indiceEscena = 1;

    [Header("Transición a usar")]
    public TransitionSettings transition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TransitionManager.Instance().Transition(
                indiceEscena,
                transition,
                0f
            );
        }
    }
}
