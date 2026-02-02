using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main menu panel with Start, Credits, and Quit buttons
/// </summary>
public class MainMenuPanel : MenuPanel
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    private MenuManager menuManager;

    protected override void Awake()
    {
        base.Awake();

        // Override positions - Main menu slides from top
        SetSlidePositions(
            hidden: new Vector2(0f, 1080f),  // Off-screen top
            visible: Vector2.zero             // Center
        );

        // Setup button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void SetMenuManager(MenuManager manager)
    {
        menuManager = manager;
    }

    private void OnStartClicked()
    {
        if (menuManager == null || menuManager.IsTransitioning) return;

        // Play button sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        menuManager.StartGame();
    }

    private void OnCreditsClicked()
    {
        if (menuManager == null || menuManager.IsTransitioning) return;

        // Play button sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        menuManager.ShowCredits();
    }

    private void OnQuitClicked()
    {
        if (menuManager == null || menuManager.IsTransitioning) return;

        // Play button sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        menuManager.QuitGame();
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartClicked);

        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(OnCreditsClicked);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
