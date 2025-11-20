using UnityEngine;

public class FlotarItem : MonoBehaviour
{
    public float velocidad = 2f;       // Velocidad del movimiento
    public float amplitud = 0.25f;     // Qué tanto sube y baja

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float nuevaY = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = posicionInicial + new Vector3(0f, nuevaY, 0f);
    }
}