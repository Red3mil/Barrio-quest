using UnityEngine;

public class ProyectilBoss : MonoBehaviour
{
    public int danio = 1;
    public float tiempoVida = 5f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            moverPersonaje player = collision.GetComponent<moverPersonaje>();
            if (player != null && !player.muerto)
            {
                Vector2 direccion = (collision.transform.position - transform.position).normalized;
                player.RecibeDanio(direccion, danio);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
