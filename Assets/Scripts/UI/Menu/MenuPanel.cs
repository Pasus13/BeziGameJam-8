using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all menu panels with slide + fade animations
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MenuPanel : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] protected float animationDuration = 0.4f;
    [SerializeField] protected AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Slide Positions")]
    [SerializeField] protected Vector2 hiddenPosition = new Vector2(1920f, 0f); // Off-screen right
    [SerializeField] protected Vector2 visiblePosition = Vector2.zero; // Center screen

    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    public bool IsAnimating { get; private set; }
    public bool IsVisible { get; private set; }

    protected virtual void Awake()
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
    public virtual void Show(bool animated = true)
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
            OnShown();
        }
    }

    /// <summary>
    /// Hide panel with animation
    /// </summary>
    public virtual void Hide(bool animated = true)
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
            OnHidden();
        }
    }

    protected virtual IEnumerator ShowCoroutine()
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
        OnShown();
    }

    protected virtual IEnumerator HideCoroutine()
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
        OnHidden();
    }

    /// <summary>
    /// Called when panel finishes showing
    /// </summary>
    protected virtual void OnShown()
    {
        Debug.Log($"<color=cyan>[MenuPanel]</color> {gameObject.name} shown");
    }

    /// <summary>
    /// Called when panel finishes hiding
    /// </summary>
    protected virtual void OnHidden()
    {
        Debug.Log($"<color=yellow>[MenuPanel]</color> {gameObject.name} hidden");
    }

    /// <summary>
    /// Set custom slide positions
    /// </summary>
    public void SetSlidePositions(Vector2 hidden, Vector2 visible)
    {
        hiddenPosition = hidden;
        visiblePosition = visible;
    }
}
