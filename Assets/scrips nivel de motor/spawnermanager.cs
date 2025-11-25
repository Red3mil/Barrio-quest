using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class spawnermanager : MonoBehaviour
{
    public List<GameObject> spawners;    
    public float tiempoPorSpawner = 5f;
    public float tiempoMuerto = 2f;

    private int indiceActual = 0;
    private float tiempoActual = 0f;
    private bool enCooldown = false;

    void Start()
    {
        ActivarSoloSpawner(indiceActual);
    }

    void Update()
    {
        if (enCooldown) return;

        tiempoActual += Time.deltaTime;

        if (tiempoActual >= tiempoPorSpawner)
        {
            tiempoActual = 0f;
            StartCoroutine(CambiarConCooldown());
        }
    }

    private IEnumerator CambiarConCooldown()
    {
        enCooldown = true;

        // Guardamos referencia al spawner para destruirlo
        GameObject spawnerAEliminar = spawners[indiceActual];

        ActivarSoloSpawner(-1);

        spawners.RemoveAt(indiceActual);
        Destroy(spawnerAEliminar);

        if (indiceActual >= spawners.Count)
            indiceActual = 0;

        yield return new WaitForSeconds(tiempoMuerto);

        if (spawners.Count > 0)
        {
            ActivarSoloSpawner(indiceActual);
        }

        enCooldown = false;
    }

    void ActivarSoloSpawner(int index)
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].SetActive(i == index);
        }
    }
}