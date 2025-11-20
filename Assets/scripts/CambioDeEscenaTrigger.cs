using UnityEngine;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class CambioDeEscenaTrigger : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string nombreEscena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Solo si el que entra es el jugador
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
}