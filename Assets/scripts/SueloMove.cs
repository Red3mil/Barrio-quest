using UnityEngine;

public class SueloMove : MonoBehaviour
{
    public float speed = 1.25f;
    public float width = 6;

    public SpriteRenderer spriteRenderer;

    private Vector2 startSize;
    
    void Start()
    {
        
        startSize = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y);


    }

   
    void Update()
    {
        spriteRenderer.size = new Vector2(spriteRenderer.size.x + speed*Time.deltaTime, spriteRenderer.size.y);

        if (spriteRenderer.size.x>width)
        {
            spriteRenderer.size = startSize;
        }




    }
}
