using System.Collections;
using UnityEngine;

/// <summary>
/// Manages menu panels and game state transitions
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private MainMenuPanel mainMenuPanel;
    [SerializeField] private CreditsPanel creditsPanel;

    [Header("Transition Settings")]
    [SerializeField] private float transitionOverlap = 0.2f;

    [Header("References")]
    [SerializeField] private GameObject player;

    public enum MenuState
    {
        MainMenu,
        Credits,
        Playing
    }

    public MenuState CurrentState { get; private set; }
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Setup panels
        if (mainMenuPanel != null)
            mainMenuPanel.SetMenuManager(this);

        if (creditsPanel != null)
            creditsPanel.SetMenuManager(this);
    }

    private void Start()
    {
        // Initial state: Show main menu
        CurrentState = MenuState.MainMenu;
        DisablePlayerInputs();

        // Play menu music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // Show main menu with animation
        if (mainMenuPanel != null)
            mainMenuPanel.Show(animated: true);

        // Ensure credits is hidden
        if (creditsPanel != null)
            creditsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Start the game (from main menu)
    /// </summary>
    public void StartGame()
    {
        if (IsTransitioning) return;

        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        IsTransitioning = true;

        // Hide main menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.Hide(animated: true);

            // Wait for animation to complete
            yield return new WaitForSecondsRealtime(mainMenuPanel.IsAnimating ? 0.4f : 0f);
        }

        // Change state
        CurrentState = MenuState.Playing;

        // Enable player
        EnablePlayerInputs();

        // Start battle music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBattleMusic();

        // Notify GameManager if exists
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStarted();

        IsTransitioning = false;

        Debug.Log("<color=green>[MenuManager]</color> Game started!");
    }

    /// <summary>
    /// Show credits panel
    /// </summary>
    public void ShowCredits()
    {
        if (IsTransitioning) return;

        StartCoroutine(ShowCreditsCoroutine());
    }

    private IEnumerator ShowCreditsCoroutine()
    {
        IsTransitioning = true;

        // Hide main menu (slide left)
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetSlidePositions(
                hidden: new Vector2(-1920f, 0f),  // Off-screen left
                visible: Vector2.zero
            );
            mainMenuPanel.Hide(animated: true);
        }

        // Wait for overlap
        yield return new WaitForSecondsRealtime(transitionOverlap);

        // Show credits (slide from right)
        if (creditsPanel != null)
        {
            creditsPanel.Show(animated: true);
        }

        // Wait for animation
        yield return new WaitForSecondsRealtime(0.4f);

        // Change state
        CurrentState = MenuState.Credits;

        IsTransitioning = false;
    }

    /// <summary>
    /// Show main menu (from credits)
    /// </summary>
    public void ShowMainMenu()
    {
        if (IsTransitioning) return;

        StartCoroutine(ShowMainMenuCoroutine());
    }

    private IEnumerator ShowMainMenuCoroutine()
    {
        IsTransitioning = true;

        // Hide credits (slide right)
        if (creditsPanel != null)
        {
            creditsPanel.SetSlidePositions(
                hidden: new Vector2(1920f, 0f),  // Off-screen right
                visible: Vector2.zero
            );
            creditsPanel.Hide(animated: true);
        }

        // Wait for overlap
        yield return new WaitForSecondsRealtime(transitionOverlap);

        // Show main menu (slide from left)
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetSlidePositions(
                hidden: new Vector2(-1920f, 0f),  // Off-screen left
                visible: Vector2.zero
            );
            mainMenuPanel.Show(animated: true);
        }

        // Wait for animation
        yield return new WaitForSecondsRealtime(0.4f);

        // Change state
        CurrentState = MenuState.MainMenu;

        IsTransitioning = false;
    }

    /// <summary>
    /// Return to main menu from gameplay
    /// </summary>
    public void ReturnToMainMenuFromGame()
    {
        if (IsTransitioning) return;

        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    private IEnumerator ReturnToMainMenuCoroutine()
    {
        IsTransitioning = true;

        // Disable player
        DisablePlayerInputs();

        // Reset player position
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.Respawn();
        }

        // Stop waves if running
        if (WaveManager.Instance != null)
            WaveManager.Instance.StopWaves();

        // Change music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // Show main menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetSlidePositions(
                hidden: new Vector2(0f, 1080f),  // Off-screen top
                visible: Vector2.zero
            );
            mainMenuPanel.Show(animated: true);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Change state
        CurrentState = MenuState.MainMenu;

        IsTransitioning = false;

        Debug.Log("<color=yellow>[MenuManager]</color> Returned to main menu");
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("<color=red>[MenuManager]</color> Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Enable player inputs
    /// </summary>
    private void EnablePlayerInputs()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.enabled = true;

        PlayerCombat pc = player.GetComponent<PlayerCombat>();
        if (pc != null)
            pc.enabled = true;

        MagicSystem ms = player.GetComponent<MagicSystem>();
        if (ms != null)
            ms.enabled = true;

        Debug.Log("<color=green>[MenuManager]</color> Player inputs enabled");
    }

    /// <summary>
    /// Disable player inputs
    /// </summary>
    private void DisablePlayerInputs()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.enabled = false;

        PlayerCombat pc = player.GetComponent<PlayerCombat>();
        if (pc != null)
            pc.enabled = false;

        MagicSystem ms = player.GetComponent<MagicSystem>();
        if (ms != null)
            ms.enabled = false;

        Debug.Log("<color=yellow>[MenuManager]</color> Player inputs disabled");
    }
}
