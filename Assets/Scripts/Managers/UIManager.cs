using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game Manager")]
    [SerializeField] private GameObject gameManager;

    [Header("Modifier Choice Panel")]
    [SerializeField] private GameObject modifierPanel;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionTitleTexts;
    [SerializeField] private TMP_Text[] optionDescTexts;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button victoryRetryButton;
    [SerializeField] private Button victoryMainMenuButton;

    private Action<int> _onOptionSelected;

    private void Awake()
    {
        Instance = this;

        if (modifierPanel != null)
            modifierPanel.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            SetupGameOverButtons();
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            SetupVictoryButtons();
        }
    }

    private void SetupGameOverButtons()
    {
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    private void SetupVictoryButtons()
    {
        if (victoryRetryButton != null)
        {
            victoryRetryButton.onClick.RemoveAllListeners();
            victoryRetryButton.onClick.AddListener(OnRetryClicked);
        }

        if (victoryMainMenuButton != null)
        {
            victoryMainMenuButton.onClick.RemoveAllListeners();
            victoryMainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    public void ShowModifierChoices(ModifierOffer[] offers, System.Action<int> onOptionSelected)
    {
        _onOptionSelected = onOptionSelected;
        modifierPanel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            optionTitleTexts[i].text = offers[i].Title;
            optionDescTexts[i].text = offers[i].Description;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() =>
            {
                modifierPanel.SetActive(false);
                _onOptionSelected?.Invoke(index);
            });
        }
    }

    public void ShowModifierPanel(System.Action onAnyButtonClicked)
    {
        modifierPanel.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() =>
            {
                modifierPanel.SetActive(false);
                Time.timeScale = 1f;
                onAnyButtonClicked?.Invoke();
            });
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("✓ Game Over Panel shown");
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("✓ Game Over Panel hidden");
        }
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("<color=yellow>✓ Victory Panel shown - You won!</color>");
        }
    }

    public void HideVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("✓ Victory Panel hidden");
        }
    }

    private void OnRetryClicked()
    {
        Debug.Log("=== RETRY BUTTON CLICKED ===");

        // 1. Ocultar panel y restaurar tiempo
        HideGameOverPanel();

        // 2. Buscar el GameManager y resetear todo el juego
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }
        else
        {
            // Fallback if no GameManager exists
            Debug.LogWarning("GameManager not found. Performing manual reset...");
            ManualReset();
        }
    }

    /// <summary>
    /// Manual reset if GameManager doesn't exist (safety fallback)
    /// </summary>
    private void ManualReset()
    {
        Debug.Log("=== STARTING MANUAL RESET ===");

        // Reset WaveManager
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.RestartWaves();
            Debug.Log("✓ WaveManager reset");
        }

        // Find player and reset components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Reset lives
            HealthComponent healthComponent = player.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.ResetHealth();
                Debug.Log("✓ Health reset");
            }

            // Reset health
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ResetForNewGame();
                Debug.Log("✓ Health reset");
            }

            // Reset position
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Respawn();
                playerMovement.ResetMultipliers();
                Debug.Log("✓ Position and multipliers reset");
            }

            // Reset combat
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            if (playerCombat != null)
            {
                playerCombat.ResetCombat();
                Debug.Log("✓ Combat reset");
            }

            // Reset magic
            MagicSystem magicSystem = player.GetComponent<MagicSystem>();
            if (magicSystem != null)
            {
                magicSystem.ResetMagic();
                Debug.Log("✓ Magic reset");
            }

            // Reset modifiers
            ModifierManager modifierManager = FindFirstObjectByType<ModifierManager>();
            if (modifierManager != null)
            {
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null)
                {
                    modifierManager.RevertAll(gm);
                    Debug.Log("✓ Modifiers reverted");
                }
            }
        }
        else
        {
            Debug.LogError("⚠ Player not found with tag 'Player'!");
        }

        Debug.Log("=== MANUAL RESET COMPLETED ===");
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        Debug.Log("Main Menu button clicked - Scene not implemented yet");
        // TODO: Implementar carga de escena del menú principal
        // SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
