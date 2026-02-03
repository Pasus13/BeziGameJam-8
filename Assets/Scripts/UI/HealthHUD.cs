using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [Header("Health Sprites")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite emptyHealthSprite;

    [Header("Settings")]
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float heartSpacing = 75f;
    [SerializeField] private Vector2 startPosition = new Vector2(25f, -15f);
    [SerializeField] private float heartSize = 64f;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private Image[] heartBackgrounds;
    private Image[] heartFills;
    private RectTransform hudRect;

    private void Awake()
    {
        hudRect = GetComponent<RectTransform>();
        CreateHearts();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthDisplay;
            UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthDisplay;
        }
    }

    private void CreateHearts()
    {
        heartBackgrounds = new Image[maxHearts];
        heartFills = new Image[maxHearts];

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartContainer = new GameObject($"Heart_{i}");
            heartContainer.transform.SetParent(transform, false);

            RectTransform containerRect = heartContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 1);
            containerRect.anchorMax = new Vector2(0, 1);
            containerRect.pivot = new Vector2(0, 1);
            containerRect.sizeDelta = new Vector2(heartSize, heartSize);
            containerRect.anchoredPosition = startPosition + new Vector2(i * heartSpacing, 0);

            GameObject backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(heartContainer.transform, false);
            RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;

            Image bgImage = backgroundObj.AddComponent<Image>();
            bgImage.sprite = backgroundSprite;
            bgImage.raycastTarget = false;
            heartBackgrounds[i] = bgImage;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(heartContainer.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchoredPosition = Vector2.zero;

            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.sprite = fullHealthSprite;
            fillImage.raycastTarget = false;
            heartFills[i] = fillImage;
        }

        Debug.Log($"<color=green>[HealthHUD]</color> Created {maxHearts} hearts");
    }

    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        int heartsToShow = Mathf.Min(currentHealth, maxHearts);

        for (int i = 0; i < maxHearts; i++)
        {
            if (i < heartsToShow)
            {
                heartFills[i].sprite = fullHealthSprite;
                heartFills[i].enabled = true;
            }
            else
            {
                heartFills[i].sprite = emptyHealthSprite;
                heartFills[i].enabled = true;
            }
        }

        Debug.Log($"<color=cyan>[HealthHUD]</color> Updated: {currentHealth}/{maxHealth} hearts");
    }

    public void SetPlayerHealth(PlayerHealth health)
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthDisplay;
        }

        playerHealth = health;

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthDisplay;
            UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }
}
