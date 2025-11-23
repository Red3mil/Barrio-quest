using UnityEngine;

public class PersonajeEnMotor : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public float velocidad = 5f;
    public float limiteSuperior = 5f;
    public float limiteInferior = -2f;

    [Header("Vida")]
    public int vidaMaxima = 10;
    public int vidaActual = 10;

    [Header("Partículas Constantes")]
    public ParticleSystem particulasConstantes;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        if (particulasConstantes != null && !particulasConstantes.isPlaying)
            particulasConstantes.Play();

        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
    }

    void Update()
    {
        float inputVertical = Input.GetAxisRaw("Vertical");

        Vector2 nuevaPos = rb.position + new Vector2(0, inputVertical * velocidad * Time.deltaTime);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, limiteInferior, limiteSuperior);

        rb.MovePosition(nuevaPos);
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    private void Morir()
    {
        
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (particulasConstantes != null)
            particulasConstantes.Stop();
    }
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstaculo"))
        {
            RecibirDanio(1); 
        }
    }
}
