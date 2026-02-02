using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Credits panel with back button
/// </summary>
public class CreditsPanel : MenuPanel
{
    [Header("Buttons")]
    [SerializeField] private Button backButton;

    private MenuManager menuManager;

    protected override void Awake()
    {
        base.Awake();

        // Override positions - Credits slides from right
        SetSlidePositions(
            hidden: new Vector2(1920f, 0f),  // Off-screen right
            visible: Vector2.zero             // Center
        );

        // Setup button listener
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    public void SetMenuManager(MenuManager manager)
    {
        menuManager = manager;
    }

    private void OnBackClicked()
    {
        if (menuManager == null || menuManager.IsTransitioning) return;

        // Play button sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        menuManager.ShowMainMenu();
    }

    private void OnDestroy()
    {
        // Clean up listener
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }
}
