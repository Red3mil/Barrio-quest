using UnityEngine;

public class ItemCuracion : MonoBehaviour
{
    public enum TipoCuracion { Parcial, RestaurarOriginal }

    [Header("Configuración")]
    public TipoCuracion tipoDeCuracion = TipoCuracion.Parcial;
    public int cantidadCuracion = 5; // Solo para el tipo Parcial

    [Header("Efectos Opcionales")]
    public AudioClip sonidoCuracion;
    public ParticleSystem particulasCuracion;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        moverPersonaje jugador = other.GetComponent<moverPersonaje>();
        if (jugador == null)
        {
            Debug.LogWarning("No se encontró el script 'moverPersonaje' en el Player. Revisa el nombre/clase.");
            return;
        }

        // Aplicar curación según el tipo
        if (tipoDeCuracion == TipoCuracion.Parcial)
            jugador.Curar(cantidadCuracion);
        else
            jugador.RestaurarVidaOriginal();

        // Reproducir sonido si existe
        if (sonidoCuracion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoCuracion, transform.position);
        }

        // Reproducir partículas si existen
        if (particulasCuracion != null)
        {
            ParticleSystem particulas = Instantiate(particulasCuracion, transform.position, Quaternion.identity);
            Destroy(particulas.gameObject, 2f);
        }

        Destroy(gameObject);
    }
}