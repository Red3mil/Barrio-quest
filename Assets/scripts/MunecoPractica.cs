using UnityEngine;

public class MunecoPractica : MonoBehaviour
{
    [Header("Configuración")]
    public int vidaMaxima = 10;
    public ParticleSystem particulasDanio;
    public AudioClip sonidoDanio;

    private Animator animator;
    private AudioSource audioSource;
    private int vida;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        vida = vidaMaxima;
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        vida -= cantDanio;

        // Activar animación de daño
        if (animator != null)
        {
            animator.SetTrigger("Danio");
        }

        // Reproducir sonido
        if (sonidoDanio != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoDanio);
        }

        // Reproducir partículas
        if (particulasDanio != null)
        {
            particulasDanio.Play();
        }

        // Si la vida llega a 0, restaurarla (es un muñeco de práctica)
        if (vida <= 0)
        {
            vida = vidaMaxima;
        }
    }
}