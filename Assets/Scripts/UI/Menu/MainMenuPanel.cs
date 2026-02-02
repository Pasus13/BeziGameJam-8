using System.Collections;
using UnityEngine;

/// <summary>
/// Menu panel with slide + fade animations
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MainMenuPanel : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Slide Positions")]
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0f, 1080f); // Off-screen top
    [SerializeField] private Vector2 visiblePosition = Vector2.zero; // Center screen

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public bool IsAnimating { get; private set; }
    public bool IsVisible { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Start hidden
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        rectTransform.anchoredPosition = hiddenPosition;
        IsVisible = false;
    }

    /// <summary>
    /// Show panel with animation
    /// </summary>
    public void Show(bool animated = true)
    {
        gameObject.SetActive(true);

        if (animated)
        {
            StartCoroutine(ShowCoroutine());
        }
        else
        {
            rectTransform.anchoredPosition = visiblePosition;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsVisible = true;
        }
    }

    /// <summary>
    /// Hide panel with animation
    /// </summary>
    public void Hide(bool animated = true)
    {
        if (animated)
        {
            StartCoroutine(HideCoroutine());
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            rectTransform.anchoredPosition = hiddenPosition;
            IsVisible = false;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowCoroutine()
    {
        IsAnimating = true;

        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            float curveValue = slideCurve.Evaluate(t);

            // Slide
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, visiblePosition, curveValue);

            // Fade
            canvasGroup.alpha = t;

            yield return null;
        }

        // Ensure final values
        rectTransform.anchoredPosition = visiblePosition;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        IsAnimating = false;
        IsVisible = true;
    }

    private IEnumerator HideCoroutine()
    {
        IsAnimating = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            float curveValue = slideCurve.Evaluate(t);

            // Slide
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, hiddenPosition, curveValue);

            // Fade
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        // Ensure final values
        rectTransform.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;

        IsAnimating = false;
        IsVisible = false;
        gameObject.SetActive(false);
    }
}