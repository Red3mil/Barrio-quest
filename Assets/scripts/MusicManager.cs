using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Referencias de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicaOriginal;
    [SerializeField] private AudioClip musicaAlternativa;

    [Header("Configuración del Prefab")]
    [SerializeField] private GameObject prefabReferencia;

    private bool prefabActivo = false;
    private GameObject prefabEnEscena;

    void Start()
    {
        // Si no asignaste el AudioSource, busca uno en la escena
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }

        // Guarda la música original si está sonando
        if (audioSource != null && audioSource.clip != null && musicaOriginal == null)
        {
            musicaOriginal = audioSource.clip;
        }

        // Reproduce la música original al inicio
        if (audioSource != null && musicaOriginal != null)
        {
            audioSource.clip = musicaOriginal;
            audioSource.Play();
        }
    }

    void Update()
    {
        // Busca si existe alguna instancia del prefab en la escena
        if (prefabReferencia != null)
        {
            prefabEnEscena = GameObject.Find(prefabReferencia.name);

            // También busca por instancias clonadas (Clone)
            if (prefabEnEscena == null)
            {
                GameObject[] todosLosObjetos = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (GameObject obj in todosLosObjetos)
                {
                    if (obj.name.Contains(prefabReferencia.name))
                    {
                        prefabEnEscena = obj;
                        break;
                    }
                }
            }
        }

        // Si el prefab apareció y no estaba activo antes
        if (prefabEnEscena != null && !prefabActivo)
        {
            CambiarMusica(musicaAlternativa);
            prefabActivo = true;
        }
        // Si el prefab desapareció y estaba activo
        else if (prefabEnEscena == null && prefabActivo)
        {
            CambiarMusica(musicaOriginal);
            prefabActivo = false;
        }
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        if (audioSource != null && nuevaMusica != null)
        {
            // Guarda el tiempo actual si quieres que continúe desde donde estaba
            // float tiempoActual = audioSource.time;

            audioSource.Stop();
            audioSource.clip = nuevaMusica;
            audioSource.Play();

            // Si quieres que continúe desde el mismo tiempo:
            // audioSource.time = tiempoActual;
        }
    }
}