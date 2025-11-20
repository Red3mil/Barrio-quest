using UnityEngine;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public TextMeshProUGUI textoPausa;   // Aquí arrastras el TMP
    [TextArea]
    public string textoQueQuieroQueDiga = "PAUSA";  // Aquí escribes lo que quieras en el Inspector

    private bool isPaused = false;

    void Start()
    {
        if (textoPausa != null)
        {
            textoPausa.text = textoQueQuieroQueDiga;
            textoPausa.gameObject.SetActive(false); // Oculta al inicio
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (textoPausa != null)
        {
            textoPausa.gameObject.SetActive(isPaused);
            textoPausa.text = textoQueQuieroQueDiga;
        }
    }
}