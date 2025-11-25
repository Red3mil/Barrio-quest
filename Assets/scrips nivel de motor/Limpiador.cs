using Unity.VisualScripting;
using UnityEngine;

public class Limpiador : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
       Destroy(gameObject);
    }
}
