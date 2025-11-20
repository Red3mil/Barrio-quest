using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;           // Velocidad del personaje
    public Transform objetivo;              // Objeto hacia el que se moverá el personaje

    [Header("Cambio de escena")]
    public string nombreEscena;             // Nombre de la escena a cargar al tocar el trigger

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (objetivo != null)
        {
            // Calcular dirección hacia el objetivo
            Vector2 direccion = (objetivo.position - transform.position).normalized;

            // Mover el personaje
            rb.linearVelocity = direccion * velocidad;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Detenerse si no hay objetivo
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar que el trigger tenga tag "TriggerScene"
        if (other.CompareTag("TriggerScene"))
        {
            // Cambiar de escena
            if (!string.IsNullOrEmpty(nombreEscena))
            {
                SceneManager.LoadScene(nombreEscena);
            }
            else
            {
                Debug.LogWarning("No se ha asignado el nombre de la escena en el Inspector.");
            }
        }
    }
}