using UnityEngine;

public class DesbloquearNivel : MonoBehaviour
{
    public int nivelADesbloquear = 2;

    void Start()
    {
        SaveSystem.SetNivelDesbloqueado(nivelADesbloquear);
    }
}

