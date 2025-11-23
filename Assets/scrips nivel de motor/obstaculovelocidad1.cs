using UnityEngine;

public class obstaculovelocidad : MonoBehaviour
{
    [Header("Rango de velocidad")]
    public float velocidadMin = 2f;
    public float velocidadMax = 6f;

    private float velocidad;

    private void Start()
    {
        velocidad = Random.Range(velocidadMin, velocidadMax);
    }

    private void Update()
    {
        // Movimiento constante hacia la izquierda
        transform.position += Vector3.left * velocidad * Time.deltaTime;
    }
}