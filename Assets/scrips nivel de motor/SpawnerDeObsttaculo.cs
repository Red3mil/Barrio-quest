using UnityEngine;
using System.Collections;
public class SpawnerDeObsttaculo : MonoBehaviour
{
    [Header("Obstáculos")]
    public GameObject[] obstaculos;   // Lista de obstáculos

    [Header("Aviso antes del spawn")]
    public GameObject avisoPrefab;    // Prefab del aviso
    public float tiempoAviso = 1f;    // Tiempo antes del spawn real

    [Header("Tiempo entre spawns")]
    public float tiempoMin = 1.5f;
    public float tiempoMax = 3.0f;

    [Header("Variación de aparición")]
    public Vector2 rangoY = new Vector2(-2f, 2f);

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Tiempo aleatorio entre spawns
            float tiempo = Random.Range(tiempoMin, tiempoMax);
            yield return new WaitForSeconds(tiempo);

            // Posición donde aparecerá el obstáculo
            Vector3 spawnPos = transform.position;
            spawnPos.y += Random.Range(rangoY.x, rangoY.y);

            // Crear aviso visual
            GameObject aviso = Instantiate(avisoPrefab, spawnPos, Quaternion.identity);

            // Esperar antes del spawn real
            yield return new WaitForSeconds(tiempoAviso);

            // Elegir obstáculo (ahora sí se puede repetir)
            int index = Random.Range(0, obstaculos.Length);

            // Instanciar obstáculo
            Instantiate(obstaculos[index], spawnPos, Quaternion.identity);

            // Eliminar aviso
            Destroy(aviso);
        }
    }
}