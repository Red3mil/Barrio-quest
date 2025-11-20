using UnityEngine;

public class CamaraDescensoSuave : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 0.1f;
    public float velocidadMax = 5f;

    [Header("Limites en Y (altura de cámara)")]
    public float limiteMinY = -3f;
    public float limiteMaxY = 5f;

    private Vector3 velocidadActual = Vector3.zero;
    private bool seguirJugador = true;
    private Vector3 posicionFija;

    void LateUpdate()
    {
        if (seguirJugador && objetivo != null)
        {
            // Sigue al jugador en X libremente
            Vector3 targetPos = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);

            // Aplica limites solo en Y
            targetPos.y = Mathf.Clamp(targetPos.y, limiteMinY, limiteMaxY);

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocidadActual, suavizado, velocidadMax);
        }
        else
        {
            // Cuando está fija, se mueve suavemente hacia la posicion fija

            Vector3 targetPos = new Vector3(posicionFija.x, posicionFija.y, transform.position.z);

            // Aplica limites solo en Y
            targetPos.y = Mathf.Clamp(targetPos.y, limiteMinY, limiteMaxY);

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocidadActual, suavizado, velocidadMax);
        }
    }

    public void BloquearEnPosicion(Vector3 pos)
    {
        seguirJugador = false;

        // Guardamos la posición fijada pero respetando límites en Y
        posicionFija = new Vector3(pos.x, Mathf.Clamp(pos.y, limiteMinY, limiteMaxY), transform.position.z);
    }

    public void SeguirJugador()
    {
        seguirJugador = true;
    }

    public Camera GetCamara()
    {
        return Camera.main;
    }
}