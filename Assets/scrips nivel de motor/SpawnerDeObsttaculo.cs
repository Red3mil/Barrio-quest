using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    // ⭐ Lista de avisos generados
    private List<GameObject> avisosActivos = new List<GameObject>();

    private void OnEnable()
    {
        rutinaSpawn = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        // Detener spawn
        if (rutinaSpawn != null)
            StopCoroutine(rutinaSpawn);

        // ⭐ Destruir todos los avisos que quedaron vivos
        LimpiarAvisos();
    }

    private void OnDestroy()
    {
        // Por si el objeto es destruido directamente
        LimpiarAvisos();
    }

    private void LimpiarAvisos()
    {
        foreach (GameObject a in avisosActivos)
        {
            if (a != null)
                Destroy(a);
        }

        avisosActivos.Clear();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float tiempo = Random.Range(tiempoMin, tiempoMax);
            yield return new WaitForSeconds(tiempo);

            Vector3 spawnPos = transform.position;
            spawnPos.y += Random.Range(rangoY.x, rangoY.y);

            // Crear aviso
            GameObject aviso = Instantiate(avisoPrefab, spawnPos, Quaternion.identity);

            // Guardarlo en lista para poder borrarlo si el spawner se destruye
            avisosActivos.Add(aviso);

            yield return new WaitForSeconds(tiempoAviso);

            int index = Random.Range(0, obstaculos.Length);
            Instantiate(obstaculos[index], spawnPos, Quaternion.identity);

            // Remover aviso de la lista y destruirlo
            avisosActivos.Remove(aviso);
            Destroy(aviso);
        }
    }
}