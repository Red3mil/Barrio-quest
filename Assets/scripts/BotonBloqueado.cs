using UnityEngine;

public class BotonBloqueado : MonoBehaviour
{
    public AudioSource sonidoError;

    public void ClickBloqueado()
    {
        if (sonidoError != null)
            sonidoError.Play();
    }
}

