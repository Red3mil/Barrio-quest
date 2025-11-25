using UnityEngine;
using System.Collections;

public class SimpleTimer : MonoBehaviour
{
    [Header("Objeto a activar/desactivar")]
    public GameObject objeto;

    [Header("Tiempo a esperar antes de reactivarlo")]
    public float tiempo = 2f;

    void Start()
    {
        if (objeto != null)
            StartCoroutine(DesactivarYActivar());
    }

    private IEnumerator DesactivarYActivar()
    {
        // Desactivar
        objeto.SetActive(false);

        // Esperar
        yield return new WaitForSeconds(tiempo);

        // Volver a activar
        objeto.SetActive(true);
    }
}
