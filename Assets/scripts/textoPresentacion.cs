using UnityEngine;
using TMPro;
using System.Collections;
public class textoPresentacion : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoUI; // Asigna el texto desde el inspector
    [SerializeField] private string mensaje = "Has activado el collider!";
    [SerializeField] private float duracionVisible = 2f; // tiempo visible
    [SerializeField] private float tiempoFade = 1f; // duración del fade
    private bool yaActivado = false; // evita múltiples activaciones

    private void Start()
    {
        if (textoUI != null)
        {
            textoUI.alpha = 0f; // invisible al inicio
            textoUI.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (yaActivado) return; // si ya se activó, no hace nada
        if (other.CompareTag("Player"))
        {
            yaActivado = true; // marca como usado
            StartCoroutine(MostrarConFade());
        }
    }

    private IEnumerator MostrarConFade()
    {
        textoUI.gameObject.SetActive(true);
        textoUI.text = mensaje;

        // 🔹 Fade In
        float t = 0f;
        while (t < tiempoFade)
        {
            t += Time.deltaTime;
            textoUI.alpha = Mathf.Lerp(0f, 1f, t / tiempoFade);
            yield return null;
        }

        textoUI.alpha = 1f;
        yield return new WaitForSeconds(duracionVisible);

        // 🔹 Fade Out
        t = 0f;
        while (t < tiempoFade)
        {
            t += Time.deltaTime;
            textoUI.alpha = Mathf.Lerp(1f, 0f, t / tiempoFade);
            yield return null;
        }

        textoUI.alpha = 0f;
        textoUI.gameObject.SetActive(false);
    }
}