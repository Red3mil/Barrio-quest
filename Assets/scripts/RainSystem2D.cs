using UnityEngine;

public class RainSystem2D : MonoBehaviour
{
    [Header("Configuración de Lluvia")]
    [SerializeField] private GameObject gotaPrefab;
    [SerializeField] private float intensidad = 50f; // Gotas por segundo
    [SerializeField] private float anchoLluvia = 20f;
    [SerializeField] private float alturaSpawn = 10f;

    [Header("Propiedades de las Gotas")]
    [SerializeField] private float velocidadCaida = 10f;
    [SerializeField] private float tiempoVida = 3f;

    private float tiempoSiguienteGota;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Ajusta automáticamente al ancho de la cámara
        if (cam != null)
        {
            anchoLluvia = cam.orthographicSize * cam.aspect * 2f;
            alturaSpawn = cam.orthographicSize + 2f;
        }
    }

    void Update()
    {
        // Calcula cuántas gotas generar este frame
        tiempoSiguienteGota -= Time.deltaTime;

        while (tiempoSiguienteGota <= 0)
        {
            GenerarGota();
            tiempoSiguienteGota += 1f / intensidad;
        }
    }

    void GenerarGota()
    {
        if (gotaPrefab == null) return;

        // Posición aleatoria en el ancho de la lluvia
        Vector3 posicion = transform.position;
        posicion.x += Random.Range(-anchoLluvia / 2f, anchoLluvia / 2f);
        posicion.y = alturaSpawn;

        // Crea la gota
        GameObject gota = Instantiate(gotaPrefab, posicion, Quaternion.identity);

        // Añade movimiento
        Rigidbody2D rb = gota.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gota.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.down * velocidadCaida;

        // Destruye después del tiempo de vida
        Destroy(gota, tiempoVida);
    }

    // Dibuja el área de lluvia en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 centro = transform.position;
        centro.y = alturaSpawn;
        Gizmos.DrawWireCube(centro, new Vector3(anchoLluvia, 0.5f, 0));
    }
}
