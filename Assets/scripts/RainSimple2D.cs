using UnityEngine;

public class RainSimple2D : MonoBehaviour
{
    [SerializeField] private GameObject gotaPrefab;
    [SerializeField] private int cantidadGotas = 50;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float anchoArea = 20f;
    [SerializeField] private float alturaArea = 15f;
    [SerializeField] private bool seguirCamara = true;

    [Header("Variación de Caída")]
    [SerializeField] private float variacionVelocidad = 1f; // Variación de velocidad entre gotas
    [SerializeField] private float velocidadHorizontal = 2f; // Movimiento horizontal (viento)
    [SerializeField] private float balanceo = 1f; // Balanceo de lado a lado
    [SerializeField] private float frecuenciaBalanceo = 2f; // Velocidad del balanceo

    private GameObject[] gotas;
    private float[] velocidadesGotas;
    private float[] faseBalanceo;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        gotas = new GameObject[cantidadGotas];
        velocidadesGotas = new float[cantidadGotas];
        faseBalanceo = new float[cantidadGotas];

        // Ajusta el área automáticamente al tamaño de la cámara
        if (cam != null && seguirCamara)
        {
            anchoArea = cam.orthographicSize * cam.aspect * 2f + 2f;
            alturaArea = cam.orthographicSize * 2f + 2f;
        }

        for (int i = 0; i < cantidadGotas; i++)
        {
            CrearGota(i);
        }
    }

    void CrearGota(int index)
    {
        Vector3 pos = transform.position + new Vector3(
            Random.Range(-anchoArea / 2, anchoArea / 2),
            Random.Range(0, alturaArea),
            0
        );

        gotas[index] = Instantiate(gotaPrefab, pos, Quaternion.identity, transform);

        // Asigna velocidad aleatoria a cada gota
        velocidadesGotas[index] = velocidad + Random.Range(-variacionVelocidad, variacionVelocidad);

        // Fase aleatoria para el balanceo
        faseBalanceo[index] = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Hace que el sistema siga a la cámara
        if (seguirCamara && cam != null)
        {
            Vector3 nuevaPos = cam.transform.position;
            nuevaPos.z = 0; // Mantiene en el plano 2D
            transform.position = nuevaPos;
        }

        for (int i = 0; i < gotas.Length; i++)
        {
            if (gotas[i] != null)
            {
                // Movimiento vertical con velocidad única
                float movimientoY = velocidadesGotas[i] * Time.deltaTime;

                // Movimiento horizontal (viento)
                float movimientoX = velocidadHorizontal * Time.deltaTime;

                // Balanceo sinusoidal
                float balanceoActual = Mathf.Sin(Time.time * frecuenciaBalanceo + faseBalanceo[i]) * balanceo * Time.deltaTime;

                gotas[i].transform.Translate(new Vector3(movimientoX + balanceoActual, -movimientoY, 0));

                // Si sale de la pantalla, la reposiciona arriba
                Vector3 posicionLocal = gotas[i].transform.position - transform.position;

                if (posicionLocal.y < -alturaArea / 2)
                {
                    Vector3 nuevaPos = transform.position;
                    nuevaPos.y += alturaArea / 2;
                    nuevaPos.x += Random.Range(-anchoArea / 2, anchoArea / 2);
                    gotas[i].transform.position = nuevaPos;

                    // Reasigna velocidad y fase aleatoria
                    velocidadesGotas[i] = velocidad + Random.Range(-variacionVelocidad, variacionVelocidad);
                    faseBalanceo[i] = Random.Range(0f, Mathf.PI * 2f);
                }
            }
        }
    }
}