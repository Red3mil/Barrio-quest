using UnityEngine;
using UnityEngine.UI;

public class MovimientoDeMotorChiquito : MonoBehaviour
{
    public Vector2 puntoA;       
    public Vector2 puntoB;       
    public float duracion = 2f;  

    private float tiempo = 0f;
    private bool moviendo = true;

    void Start()
    {
        transform.position = puntoA;
    }

    void Update()
    {
        if (!moviendo) return;

        tiempo += Time.deltaTime;
        float t = tiempo / duracion;

        // Clamp para evitar pasar el 1
        t = Mathf.Clamp01(t);

        transform.position = Vector2.Lerp(puntoA, puntoB, t);

        // Cuando llega a destino, puedes detenerlo
        if (t >= 1f)
        {
            moviendo = false;
        }
    }
}