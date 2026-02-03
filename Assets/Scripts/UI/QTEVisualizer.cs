using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEVisualizer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private RectTransform indicatorBar;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Background")]
    [SerializeField] private Image lineBackgroundImage; // Tablatura sprite

    [Header("Arrow Sprites")]
    [SerializeField] private Sprite upArrowSprite;
    [SerializeField] private Sprite downArrowSprite;
    [SerializeField] private Sprite rightArrowSprite;
    [SerializeField] private Sprite leftArrowSprite;

    [Header("Feedback Sprites")]
    [SerializeField] private Sprite perfectRingSprite; // Aro para Perfect
    [SerializeField] private Sprite goodRingSprite;    // Aro para Good (puede ser el mismo)

    [Header("Feedback Settings")]
    [SerializeField] private float feedbackDuration = 0.5f;
    [SerializeField] private Color perfectColor = Color.green;
    [SerializeField] private Color goodColor = Color.yellow;
    [SerializeField] private Color missColor = Color.red;

    private List<GameObject> buttonVisuals = new List<GameObject>();
    private Dictionary<GameObject, Image> feedbackRings = new Dictionary<GameObject, Image>();
    private QTEManager qteManager;
    private bool hasSetupButtons = false;
    private int lastCheckedButtonIndex = -1;

    private void Awake()
    {
        AutoFindReferences();

        if (qtePanel != null)
        {
            qtePanel.SetActive(false);
            Debug.Log("<color=cyan>[QTEVisualizer]</color> QTE Panel initialized and hidden");
        }
        else
        {
            Debug.LogError("<color=red>[QTEVisualizer]</color> QTE Panel not found!");
        }
    }

    private void Start()
    {
        if (qteManager == null)
        {
            qteManager = FindFirstObjectByType<QTEManager>();
            
            if (qteManager == null)
            {
                Debug.LogError("<color=red>[QTEVisualizer]</color> QTEManager not found in scene!");
            }
            else
            {
                Debug.Log("<color=cyan>[QTEVisualizer]</color> QTEManager found successfully");
            }
        }

        ValidateSprites();
        InitializeIndicatorSprite();
    }

    private void ValidateSprites()
    {
        if (upArrowSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Up Arrow Sprite not assigned!");
        if (downArrowSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Down Arrow Sprite not assigned!");
        if (rightArrowSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Right Arrow Sprite not assigned!");
        if (leftArrowSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Left Arrow Sprite not assigned!");
        if (perfectRingSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Perfect Ring Sprite not assigned!");
        if (goodRingSprite == null) Debug.LogWarning("<color=yellow>[QTEVisualizer]</color> Good Ring Sprite not assigned!");
    }

    private void InitializeIndicatorSprite()
    {
        if (indicatorBar != null && perfectRingSprite != null)
        {
            Image indicatorImage = indicatorBar.GetComponent<Image>();
            if (indicatorImage != null)
            {
                indicatorImage.sprite = perfectRingSprite;
                indicatorImage.color = Color.white;
                indicatorImage.preserveAspect = true;
                Debug.Log($"<color=cyan>[QTEVisualizer]</color> Indicator sprite initialized: {perfectRingSprite.name}");
            }
        }
        else
        {
            if (perfectRingSprite == null)
            {
                Debug.LogError("<color=red>[QTEVisualizer]</color> Failed to initialize indicator - perfectRingSprite is null!");
            }
        }
    }

    private void AutoFindReferences()
    {
        if (qtePanel == null)
        {
            Transform panel = transform.Find("QTE Panel");
            if (panel != null) qtePanel = panel.gameObject;
        }

        if (indicatorBar == null)
        {
            Transform indicator = transform.Find("QTE Panel/Indicator");
            if (indicator != null) indicatorBar = indicator.GetComponent<RectTransform>();
        }

        if (buttonsContainer == null)
        {
            Transform container = transform.Find("QTE Panel/Buttons Container");
            if (container != null) buttonsContainer = container;
        }

        if (buttonPrefab == null)
        {
            Transform prefab = transform.Find("Button Prefab");
            if (prefab != null) buttonPrefab = prefab.gameObject;
        }

        if (lineBackgroundImage == null)
        {
            Transform bg = transform.Find("QTE Panel/Bar Background");
            if (bg != null) lineBackgroundImage = bg.GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (qteManager == null || !qteManager.IsQTEActive())
        {
            if (qtePanel != null && qtePanel.activeSelf)
            {
                HideQTE();
            }
            hasSetupButtons = false;
            lastCheckedButtonIndex = -1;
            return;
        }

        if (qtePanel != null && !qtePanel.activeSelf)
        {
            ShowQTE();
        }

        UpdateProgressBar();
        CheckForCompletedButtons();
    }

    private void ShowQTE()
    {
        if (qtePanel != null)
            qtePanel.SetActive(true);
    }

    private void HideQTE()
    {
        if (qtePanel != null)
            qtePanel.SetActive(false);
        ClearButtonVisuals();
    }

    private void UpdateProgressBar()
    {
        float progress = qteManager.GetCurrentProgress();

        if (indicatorBar != null)
        {
            Vector2 anchoredPos = indicatorBar.anchoredPosition;
            anchoredPos.x = Mathf.Lerp(-400f, 400f, progress);
            indicatorBar.anchoredPosition = anchoredPos;
        }
    }

    /// <summary>
    /// Setup buttons with arrow sprites
    /// </summary>
    public void SetupButtons(List<QTEButton> buttons)
    {
        if (hasSetupButtons) return;

        ClearButtonVisuals();

        if (buttonsContainer == null || buttonPrefab == null)
        {
            Debug.LogError("<color=red>[QTEVisualizer]</color> Cannot setup buttons - missing buttonsContainer or buttonPrefab!");
            return;
        }

        Debug.Log($"<color=cyan>[QTEVisualizer]</color> Setting up {buttons.Count} buttons...");

        foreach (QTEButton button in buttons)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonsContainer);
            buttonObj.SetActive(true);

            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = buttonObj.AddComponent<RectTransform>();
            }

            float xPos = Mathf.Lerp(-400f, 400f, button.targetPosition);
            rectTransform.anchoredPosition = new Vector2(xPos, 0f);
            rectTransform.sizeDelta = new Vector2(80f, 80f);
            rectTransform.localScale = Vector3.one;

            Image arrowImage = buttonObj.GetComponent<Image>();
            if (arrowImage == null)
            {
                arrowImage = buttonObj.AddComponent<Image>();
            }

            Sprite arrowSprite = GetSpriteForKeyCode(button.keyCode);
            if (arrowSprite != null)
            {
                arrowImage.sprite = arrowSprite;
                arrowImage.color = Color.white;
                arrowImage.preserveAspect = true;
                arrowImage.raycastTarget = false;
                
                Debug.Log($"<color=green>[QTEVisualizer]</color> Button created: name='{buttonObj.name}', pos=({xPos:F0}, 0), size=(80x80), sprite='{arrowSprite.name}', active={buttonObj.activeSelf}");
            }
            else
            {
                Debug.LogWarning($"<color=yellow>[QTEVisualizer]</color> No sprite found for {button.keyCode}, using red placeholder");
                arrowImage.color = new Color(1f, 0f, 0f, 1f);
            }

            GameObject ringObj = new GameObject("FeedbackRing");
            ringObj.transform.SetParent(buttonObj.transform, false);
            ringObj.transform.localPosition = Vector3.zero;
            ringObj.transform.localScale = Vector3.one * 1.2f;

            Image ringImage = ringObj.AddComponent<Image>();
            ringImage.sprite = perfectRingSprite;
            ringImage.color = new Color(1f, 1f, 1f, 0f);
            ringImage.raycastTarget = false;

            RectTransform ringRect = ringObj.GetComponent<RectTransform>();
            ringRect.sizeDelta = rectTransform.sizeDelta * 1.3f;

            feedbackRings[buttonObj] = ringImage;
            buttonVisuals.Add(buttonObj);
        }

        hasSetupButtons = true;

        Debug.Log($"<color=cyan>[QTEVisualizer]</color> Setup complete! {buttons.Count} buttons created with sprites.");
    }

    /// <summary>
    /// Get the correct arrow sprite for a KeyCode
    /// </summary>
    private Sprite GetSpriteForKeyCode(KeyCode keyCode)
    {
        Sprite sprite = null;

        switch (keyCode)
        {
            case KeyCode.UpArrow:
                sprite = upArrowSprite;
                break;

            case KeyCode.DownArrow:
                sprite = downArrowSprite;
                break;

            case KeyCode.RightArrow:
                sprite = rightArrowSprite;
                break;

            case KeyCode.LeftArrow:
                sprite = leftArrowSprite;
                break;

            default:
                Debug.LogWarning($"<color=yellow>[QTEVisualizer]</color> No sprite mapping for KeyCode: {keyCode}");
                return null;
        }

        if (sprite == null)
        {
            Debug.LogError($"<color=red>[QTEVisualizer]</color> Sprite for {keyCode} is NULL! Check Inspector assignments or LoadSpritesIfNeeded()");
        }

        return sprite;
    }

    private void CheckForCompletedButtons()
    {
        int currentIndex = qteManager.GetCurrentButtonIndex();

        // If button index advanced, previous button was completed
        if (currentIndex > lastCheckedButtonIndex && lastCheckedButtonIndex >= 0)
        {
            // Show feedback for the completed button
            if (lastCheckedButtonIndex < buttonVisuals.Count)
            {
                ShowButtonFeedback(lastCheckedButtonIndex);
            }
        }

        lastCheckedButtonIndex = currentIndex;
    }

    /// <summary>
    /// Show feedback ring (Aro) around completed button
    /// </summary>
    private void ShowButtonFeedback(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonVisuals.Count) return;

        GameObject buttonObj = buttonVisuals[buttonIndex];

        if (feedbackRings.TryGetValue(buttonObj, out Image ringImage))
        {
            int score = qteManager.GetLastButtonScore();
            
            Color feedbackColor = missColor;
            
            if (score == 3)
            {
                feedbackColor = perfectColor;
            }
            else if (score == 1)
            {
                feedbackColor = goodColor;
            }

            StartCoroutine(AnimateFeedbackRing(ringImage, feedbackColor));
        }
    }

    /// <summary>
    /// Animate the feedback ring (fade in, scale, fade out)
    /// </summary>
    private IEnumerator AnimateFeedbackRing(Image ringImage, Color feedbackColor)
    {
        if (ringImage == null) yield break;

        // Set initial state
        ringImage.color = feedbackColor;
        ringImage.transform.localScale = Vector3.one * 0.8f;

        float elapsed = 0f;
        float halfDuration = feedbackDuration / 2f;

        // Fade in + scale up
        while (elapsed < halfDuration)
        {
            if (ringImage == null) yield break;
            
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / halfDuration;

            float alpha = Mathf.Lerp(0f, 1f, t);
            ringImage.color = new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, alpha);

            float scale = Mathf.Lerp(0.8f, 1.2f, t);
            ringImage.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Hold for a moment
        yield return new WaitForSecondsRealtime(0.1f);

        if (ringImage == null) yield break;

        // Fade out
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            if (ringImage == null) yield break;
            
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / halfDuration;

            float alpha = Mathf.Lerp(1f, 0f, t);
            ringImage.color = new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, alpha);

            yield return null;
        }

        // Ensure fully transparent
        if (ringImage != null)
        {
            ringImage.color = new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, 0f);
        }
    }

    private void ClearButtonVisuals()
    {
        foreach (GameObject buttonObj in buttonVisuals)
        {
            if (buttonObj != null)
                Destroy(buttonObj);
        }

        buttonVisuals.Clear();
        feedbackRings.Clear();
        hasSetupButtons = false;
    }
}