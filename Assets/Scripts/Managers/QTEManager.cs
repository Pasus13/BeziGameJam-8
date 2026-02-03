using System;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    [Header("QTE Settings")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float barDuration = 2f;
    
    [Header("Zone Thresholds")]
    [SerializeField] private float perfectZoneSize = 0.05f;
    [SerializeField] private float goodZoneSize = 0.10f;

    public event Action<int> OnQTEComplete;
    
    private List<QTEButton> qteButtons;
    private QTEVisualizer visualizer;

    private int currentButtonIndex;
    private int totalScore;
    private int lastButtonScore;
    private float currentProgress;
    private float qteTimer;
    private float perfectZoneMultiplier = 1f;  // Modified by modifiers
    private float goodZoneMultiplier = 1f;     // Modified by modifiers
    private bool isQTEActive;
    private AudioClip pendingQTEAudio;
    private bool audioPlayed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        visualizer = FindFirstObjectByType<QTEVisualizer>();
    }

    private void Update()
    {
        if (!isQTEActive) return;

        qteTimer += Time.unscaledDeltaTime;

        if (qteTimer < startDelay)
        {
            currentProgress = 0f;
            return;
        }

        if (!audioPlayed && pendingQTEAudio != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.SFX.PlaySFX(pendingQTEAudio);
            Debug.Log($"<color=cyan>[QTEManager]</color> Playing QTE audio: {pendingQTEAudio.name}");
            audioPlayed = true;
        }

        float effectiveTime = qteTimer - startDelay;
        currentProgress = effectiveTime / barDuration;

        if (currentProgress >= 1f)
        {
            EndQTE();
            return;
        }

        CheckInput();
    }

    public void StartQTE(List<QTEButton> buttons, AudioClip qteAudio = null)
    {
        qteButtons = buttons;
        currentButtonIndex = 0;
        totalScore = 0;
        qteTimer = 0f;
        currentProgress = 0f;
        isQTEActive = true;
        pendingQTEAudio = qteAudio;
        audioPlayed = false;

        Time.timeScale = 0f;

        PauseGame();

        if (visualizer != null)
        {
            visualizer.SetupButtons(buttons);
        }

        Debug.Log($"QTE Started with {buttons.Count} buttons!");
    }

    private void CheckInput()
    {
        if (currentButtonIndex >= qteButtons.Count) return;

        QTEButton currentButton = qteButtons[currentButtonIndex];

        // CAMBIAR A FLECHAS:
        bool up_pressed = InputManager.QTEButtonUpWasPressed;     
        bool down_pressed = InputManager.QTEButtonDownWasPressed; 
        bool right_pressed = InputManager.QTEButtonRightWasPressed;
        bool left_pressed = InputManager.QTEButtonLeftWasPressed;  

        bool anyButtonPressed = up_pressed || down_pressed || right_pressed || left_pressed;

        if (!anyButtonPressed) return;

        bool correctButtonPressed =
            (up_pressed && currentButton.keyCode == KeyCode.UpArrow) ||
            (down_pressed && currentButton.keyCode == KeyCode.DownArrow) ||
            (right_pressed && currentButton.keyCode == KeyCode.RightArrow) ||
            (left_pressed && currentButton.keyCode == KeyCode.LeftArrow);

        if (correctButtonPressed)
        {
            int score = EvaluateHit(currentButton.targetPosition);
            totalScore += score;
            lastButtonScore = score;

            string result = score == 3 ? "PERFECTO" : score == 1 ? "PARCIAL" : "MISS";
            Debug.Log($"Button {currentButtonIndex + 1}: {result} (+{score} puntos)");

            currentButtonIndex++;
        }
        else
        {
            string wrongKey = up_pressed ? "↑" : down_pressed ? "↓" : right_pressed ? "→" : "←";
            string expectedKey = currentButton.keyCode.ToString();

            Debug.Log($"Button {currentButtonIndex + 1}: FALLO - Presionaste {wrongKey} (esperaba {expectedKey}) (+0 puntos)");

            lastButtonScore = 0;
            currentButtonIndex++;
        }
    }

    private int EvaluateHit(float targetPosition)
    {
        float distance = Mathf.Abs(currentProgress - targetPosition);

        // Apply multipliers to zone sizes
        float effectivePerfectZone = perfectZoneSize * perfectZoneMultiplier;
        float effectiveGoodZone = goodZoneSize * goodZoneMultiplier;

        if (distance <= effectivePerfectZone)
        {
            return 3; // Perfect
        }
        else if (distance <= effectiveGoodZone)
        {
            return 1; // Good
        }
        else
        {
            return 0; // Miss
        }
    }

    private void EndQTE()
    {
        isQTEActive = false;

        Time.timeScale = 1f;

        ResumeGame();
        
        Debug.Log($"=== MAGIA FINALIZADA === Puntos obtenidos: {totalScore}/{qteButtons.Count * 3}");
        
        OnQTEComplete?.Invoke(totalScore);
    }

    private void PauseGame()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }
    }

    private void ResumeGame()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.enabled = true;
        }
    }

    public bool IsQTEActive()
    {
        return isQTEActive;
    }

    public float GetCurrentProgress()
    {
        return currentProgress;
    }

    public int GetCurrentButtonIndex()
    {
        return currentButtonIndex;
    }

    public int GetLastButtonScore()
    {
        return lastButtonScore;
    }

    /// <summary>
    /// Get current perfect zone size multiplier (for modifiers)
    /// </summary>
    public float GetPerfectZoneMultiplier()
    {
        return perfectZoneMultiplier;
    }

    /// <summary>
    /// Set perfect zone size multiplier (for modifiers)
    /// </summary>
    public void SetPerfectZoneMultiplier(float multiplier)
    {
        perfectZoneMultiplier = Mathf.Max(0.1f, multiplier); // Min 0.1x (10% of normal)
        Debug.Log($"<color=magenta>[QTEManager]</color> Perfect zone multiplier: {perfectZoneMultiplier:F2}x");
    }

    /// <summary>
    /// Get current good zone size multiplier (for modifiers - optional)
    /// </summary>
    public float GetGoodZoneMultiplier()
    {
        return goodZoneMultiplier;
    }

    /// <summary>
    /// Set good zone size multiplier (for modifiers - optional)
    /// </summary>
    public void SetGoodZoneMultiplier(float multiplier)
    {
        goodZoneMultiplier = Mathf.Max(0.1f, multiplier);
        Debug.Log($"<color=magenta>[QTEManager]</color> Good zone multiplier: {goodZoneMultiplier:F2}x");
    }
}

[Serializable]
public class QTEButton
{
    public KeyCode keyCode;
    public float targetPosition;

    public QTEButton(KeyCode key, float position)
    {
        keyCode = key;
        targetPosition = position;
    }
}
