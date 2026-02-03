using UnityEngine;

public class VFXLifetimeController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.7f;

    private SpriteRenderer spriteRenderer;
    private float lifetime;
    private float elapsedTime;
    private Color originalColor;
    private bool isActive;

    public void Initialize(float totalDuration)
    {
        lifetime = totalDuration;
        elapsedTime = 0f;
        isActive = true;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            Color startColor = originalColor;
            startColor.a = 0f;
            spriteRenderer.color = startColor;
        }
    }

    private void Update()
    {
        if (!isActive || spriteRenderer == null || lifetime <= 0f) return;

        elapsedTime += Time.deltaTime;

        float alpha = CalculateAlpha();
        Color newColor = originalColor;
        newColor.a = alpha;
        spriteRenderer.color = newColor;

        if (elapsedTime >= lifetime)
        {
            isActive = false;
            gameObject.SetActive(false);
        }
    }

    private float CalculateAlpha()
    {
        if (elapsedTime < fadeInDuration)
        {
            return Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
        }

        float fadeOutStartTime = lifetime - fadeOutDuration;
        if (elapsedTime >= fadeOutStartTime)
        {
            float fadeOutProgress = (elapsedTime - fadeOutStartTime) / fadeOutDuration;
            return Mathf.Lerp(1f, 0f, fadeOutProgress);
        }

        return 1f;
    }
}
