using UnityEngine;
using TMPro;

public class MostrarTextoTrigger : MonoBehaviour
{
    [Header("Texto que quieres mostrar")]
    [TextArea]
    public string mensaje;

    [Header("Referencia al TextMeshProUGUI")]
    public TextMeshProUGUI textoUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate que el jugador tenga el tag "Player"
        {
            textoUI.text = mensaje;
            textoUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoUI.gameObject.SetActive(false);
        }
    }
}