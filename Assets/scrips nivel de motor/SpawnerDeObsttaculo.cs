using UnityEngine;
using System.Collections;
public class SpawnerDeObsttaculo : MonoBehaviour
{
    [Header("Obstáculos")]
    public GameObject[] obstaculos;

    [Header("Aviso antes del spawn")]
    public GameObject avisoPrefab;
    public float tiempoAviso = 1f;

    [Header("Tiempo entre spawns")]
    public float tiempoMin = 1.5f;
    public float tiempoMax = 3.0f;

    [Header("Variación de aparición")]
    public Vector2 rangoY = new Vector2(-2f, 2f);

    private Coroutine rutinaSpawn;

    private void OnEnable()
    {
        // Cuando el spawner se active, iniciar la corrutina
        rutinaSpawn = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        // Cuando se desactive, detener corrutina para evitar errores
        if (rutinaSpawn != null)
            StopCoroutine(rutinaSpawn);
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

            // Elegir obstáculo
            int index = Random.Range(0, obstaculos.Length);

            // Instanciar obstáculo
            Instantiate(obstaculos[index], spawnPos, Quaternion.identity);

            // Eliminar aviso
            Destroy(aviso);
        }
    }
}