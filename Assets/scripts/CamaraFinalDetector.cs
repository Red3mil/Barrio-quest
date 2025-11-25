using UnityEngine;

public class CamaraFinalDetector : MonoBehaviour
{
    [Header("Referencia a la cámara con movimiento")]
    public CamaraDescensoSuave camara;

    [Header("Posición exacta donde debe llegar la cámara")]
    public Vector3 posicionObjetivo;

    [Header("Qué tan exacto debe ser (0.05 = muy preciso)")]
    public float tolerancia = 0.05f;

    [Header("Selector de nivel a habilitar")]
    public LevelSelector levelSelector;

    private bool yaActivado = false;

    void Update()
    {
        if (yaActivado) return;

        Vector3 camPos = camara.transform.position;

        bool llego =
            Mathf.Abs(camPos.x - posicionObjetivo.x) <= tolerancia &&
            Mathf.Abs(camPos.y - posicionObjetivo.y) <= tolerancia;

        if (llego)
        {
            yaActivado = true;

            // Avisamos al LevelSelector que ya puede ser abierto
            levelSelector.PermitirApertura();
        }
    }
}
