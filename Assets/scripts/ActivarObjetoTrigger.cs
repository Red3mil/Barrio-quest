using UnityEngine;
using System.Collections;

public class ActivarObjetoTrigger2D : MonoBehaviour
{
    [Header("Objeto a Activar")]
    public GameObject objetoActivar; // Arrástralo en el inspector

    [Header("Sonido al Activar")]
    public AudioSource audioSource; // Arrastra el AudioSource aquí

    [Header("Efecto de Temblor")]
    public Camera camara; // Arrastra la cámara principal aquí
    public float duracionTemblor = 0.2f;
    public float fuerzaTemblor = 0.2f;

    private Vector3 posicionOriginalCamara;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objetoActivar != null)
                objetoActivar.SetActive(true);

            if (audioSource != null)
                audioSource.Play();

            if (camara != null)
                StartCoroutine(TemblorCamara());
        }
    }

    IEnumerator TemblorCamara()
    {
        posicionOriginalCamara = camara.transform.localPosition;

        float tiempo = 0f;
        while (tiempo < duracionTemblor)
        {
            float x = Random.Range(-1f, 1f) * fuerzaTemblor;
            float y = Random.Range(-1f, 1f) * fuerzaTemblor;

            camara.transform.localPosition = new Vector3(
                posicionOriginalCamara.x + x,
                posicionOriginalCamara.y + y,
                posicionOriginalCamara.z
            );

            tiempo += Time.deltaTime;
            yield return null;
        }

        camara.transform.localPosition = posicionOriginalCamara;
    }
}