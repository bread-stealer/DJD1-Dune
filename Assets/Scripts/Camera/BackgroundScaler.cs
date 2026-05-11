using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float spriteHeight = sr.bounds.size.y;
        float spriteWidth = sr.bounds.size.x;

        transform.localScale = new Vector3(
            width / spriteWidth,
            height / spriteHeight,
            1f
        );
    }
}