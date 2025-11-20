using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscenaPorIndice : MonoBehaviour
{
    [Tooltip("Índice de la escena a cargar. En tu caso: 1")]
    public int indiceEscena = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter
        {
            SceneManager.LoadScene(indiceEscena);
        }
    }
}