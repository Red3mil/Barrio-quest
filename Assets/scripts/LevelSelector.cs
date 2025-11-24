using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject canvasSelector;        // El menú de niveles
    public GameObject canvasPressEnter;      // El texto "Press Enter"

    [Header("Botones de niveles")]
    public GameObject botonNivel1;
    public GameObject botonNivel2;
    public GameObject botonNivel3;

    [Header("Iconos bloqueados")]
    public GameObject bloqueado2;
    public GameObject bloqueado3;

    [Header("Configuración")]
    public float tiempoParaActivar = 2f;

    private bool camaraListo = false;
    private bool tiempoListo = false;

    void Start()
    {
        canvasSelector.SetActive(false);

        ActualizarBotones();

        // Activación por tiempo pero NO permite abrir hasta que la cámara también lo apruebe
        Invoke(nameof(ActivarPorTiempo), tiempoParaActivar);
    }

    void Update()
    {
        if (camaraListo && tiempoListo && Input.GetKeyDown(KeyCode.Return))
            AbrirSelector();
    }

    void ActivarPorTiempo()
    {
        tiempoListo = true;
    }

    // Esta función la llama la cámara cuando llega
    public void PermitirApertura()
    {
        camaraListo = true;
    }

    void AbrirSelector()
    {
        canvasSelector.SetActive(true);
        canvasPressEnter.SetActive(false);
    }

    public void ActualizarBotones()
    {
        botonNivel1.SetActive(true);

        bool n2 = SaveSystem.EstaDesbloqueado(2);
        botonNivel2.SetActive(n2);
        bloqueado2.SetActive(!n2);

        bool n3 = SaveSystem.EstaDesbloqueado(3);
        botonNivel3.SetActive(n3);
        bloqueado3.SetActive(!n3);
    }
}
