using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Modifier Choice Panel")]
    [SerializeField] private GameObject modifierPanel;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionTitleTexts;
    [SerializeField] private TMP_Text[] optionDescTexts;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

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
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnRetryClicked()
    {
        HideGameOverPanel();
        
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.RestartWaves();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.ResetLives();
            }
        }
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        Debug.Log("Main Menu button clicked - Scene not implemented yet");
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
