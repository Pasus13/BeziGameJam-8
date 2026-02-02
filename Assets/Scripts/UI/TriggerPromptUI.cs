using UnityEngine;
using TMPro;

/// <summary>
/// UI that shows prompt messages when player is near triggers
/// </summary>
public class TriggerPromptUI : MonoBehaviour
{
    public static TriggerPromptUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;
    private bool isVisible = false;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get or add CanvasGroup
        if (promptPanel != null)
        {
            canvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = promptPanel.AddComponent<CanvasGroup>();
            }
        }

        // Start hidden
        HidePrompt();
    }

    /// <summary>
    /// Show prompt with custom message
    /// </summary>
    public void ShowPrompt(string message)
    {
        if (promptPanel == null || promptText == null) return;

        promptText.text = message;
        promptPanel.SetActive(true);
        isVisible = true;

        // Fade in
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            LeanTween.alphaCanvas(canvasGroup, 1f, 1f / fadeSpeed).setIgnoreTimeScale(true);
        }

        Debug.Log($"<color=cyan>[TriggerPrompt]</color> Showing: {message}");
    }

    /// <summary>
    /// Hide prompt
    /// </summary>
    public void HidePrompt()
    {
        if (promptPanel == null) return;

        isVisible = false;

        // Fade out
        if (canvasGroup != null)
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, 1f / fadeSpeed)
                .setIgnoreTimeScale(true)
                .setOnComplete(() => promptPanel.SetActive(false));
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }

    public bool IsVisible => isVisible;
}
