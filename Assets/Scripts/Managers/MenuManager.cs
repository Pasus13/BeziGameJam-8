using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages main menu and game state transitions
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Panel")]
    [SerializeField] private MainMenuPanel mainMenuPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("References")]
    [SerializeField] private GameObject player;

    public bool IsInMenu { get; private set; }
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

        // Setup button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
            Debug.Log("<color=cyan>[MenuManager]</color> Start button listener added");
        }
        else
        {
            Debug.LogError("<color=red>[MenuManager]</color> Start button is NULL!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            Debug.Log("<color=cyan>[MenuManager]</color> Quit button listener added");
        }
        else
        {
            Debug.LogError("<color=red>[MenuManager]</color> Quit button is NULL!");
        }
    }

    private void Start()
    {
        // Initial state: Show main menu
        IsInMenu = true;
        DisablePlayerInputs();

        // Play menu music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // Show menu with animation
        if (mainMenuPanel != null)
        {
            mainMenuPanel.gameObject.SetActive(true); // ← AÑADIR ESTO
            mainMenuPanel.Show(animated: true);
        }
    }

    public void OnStartClicked()
    {
        Debug.Log("<color=green>[MenuManager]</color> ========== START CLICKED ==========");
        Debug.Log($"<color=green>[MenuManager]</color> IsTransitioning: {IsTransitioning}");

        if (IsTransitioning)
        {
            Debug.Log("<color=yellow>[MenuManager]</color> Already transitioning, ignoring click");
            return;
        }

        Debug.Log("<color=green>[MenuManager]</color> Playing button click sound...");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        Debug.Log("<color=green>[MenuManager]</color> Calling StartGame()...");
        StartGame();
    }

    public void OnQuitClicked()
    {
        if (IsTransitioning) return;

        // Play button sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        QuitGame();
    }

    public void StartGame()
    {
        Debug.Log("<color=cyan>[MenuManager]</color> ========== StartGame() CALLED ==========");

        if (IsTransitioning)
        {
            Debug.Log("<color=yellow>[MenuManager]</color> Already transitioning in StartGame");
            return;
        }

        Debug.Log("<color=cyan>[MenuManager]</color> Starting coroutine...");
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {

        IsTransitioning = true;

        // Hide menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.Hide(animated: true);

            // Wait for animation to complete
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Change state
        IsInMenu = false;

        // Enable player
        EnablePlayerInputs();

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted();
        }

        IsTransitioning = false;
    }

    /// <summary>
    /// Return to main menu from gameplay
    /// </summary>
    public void ReturnToMainMenuFromGame()
    {
        if (IsTransitioning) return;

        StartCoroutine(ReturnToMenuCoroutine());
    }

    private IEnumerator ReturnToMenuCoroutine()
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

        // Stop waves
        if (WaveManager.Instance != null)
            WaveManager.Instance.StopWaves();

        // Change music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // Show menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.Show(animated: true);
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Change state
        IsInMenu = true;

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

    private void OnDestroy()
    {
        // Clean up listeners
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartClicked);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}