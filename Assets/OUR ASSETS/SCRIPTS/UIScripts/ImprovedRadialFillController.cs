using UnityEngine;

public class ImprovedRadialFillController : MonoBehaviour
{
    public float fillDuration = 2f; // Duration to fill the sprite
    public float startAngle = 0f; // Start angle in degrees
    private Material material;
    private float fillAmount = 0f;
    private float elapsedTime = 0f;

    void Start()
    {
        InitMaterial();
    }

    public void InitMaterial()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.SetFloat("_FillAmount", 0f);
        material.SetVector("_Center", new Vector4(0.5f, 0.5f, 0, 0));
        material.SetFloat("_StartAngle", startAngle / 360f); // Convert start angle to range [0,1]
        fillAmount = 0;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (fillAmount < 1f)
        {
            elapsedTime += Time.deltaTime;
            fillAmount = Mathf.Clamp01(elapsedTime / fillDuration);
            material.SetFloat("_FillAmount", fillAmount);
        }
    }
}
