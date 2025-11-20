using UnityEngine;
using System.Collections;

public class CamaraShake : MonoBehaviour
{
    // Método público que puedes llamar desde el personaje
    public void Shake(float duracion, float intensidad)
    {
        StartCoroutine(ShakeCoroutine(duracion, intensidad));
    }

    private IEnumerator ShakeCoroutine(float duracion, float intensidad)
    {
        Vector3 posOriginal = transform.position;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracion)
        {
            float offsetX = Random.Range(-1f, 1f) * intensidad;
            float offsetY = Random.Range(-1f, 1f) * intensidad;

            transform.position = new Vector3(posOriginal.x + offsetX, posOriginal.y + offsetY, posOriginal.z);

            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        // Al finalizar, regresamos a la posición original para no desalinear el follow
        transform.position = posOriginal;
    }
}