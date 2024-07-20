using UnityEngine;

public class RadialMotion : MonoBehaviour
{
    public float appearTime = 1f; // Time for the sprite to fully appear
    public float radius = 5f; // Radius of the circular motion
    public float speed = 1f; // Speed of the circular motion

    private SpriteRenderer spriteRenderer;
    private float elapsedTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Start with the sprite invisible
    }

    void Update()
    {
        // Appear animation
        if (elapsedTime < appearTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / appearTime);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
        }

        // Radial motion
        float angle = speed * Time.time; // Angle increases over time
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
