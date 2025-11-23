using UnityEngine;

public class spawnermanager : MonoBehaviour
{
    public GameObject[] spawners;       // Lista de spawners
    public float tiempoPorSpawner = 5f; // Tiempo que un spawner permanece activo
    public float tiempoMuerto = 2f;     // Tiempo sin spawner activo antes del siguiente

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

    private System.Collections.IEnumerator CambiarConCooldown()
    {
        enCooldown = true;

        // 1. Apagar todos durante el cooldown
        ActivarSoloSpawner(-1);

        // 2. Esperar tiempo muerto
        yield return new WaitForSeconds(tiempoMuerto);

        // 3. Pasar al siguiente spawner
        CambiarSpawner();

        enCooldown = false;
    }

    void CambiarSpawner()
    {
        indiceActual++;

        if (indiceActual >= spawners.Length)
            indiceActual = 0;

        ActivarSoloSpawner(indiceActual);
    }

    void ActivarSoloSpawner(int index)
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].SetActive(i == index);
        }
    }
}