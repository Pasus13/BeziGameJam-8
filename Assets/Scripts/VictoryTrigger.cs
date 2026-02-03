using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private BoxCollider2D doorCollider;
    
    [Header("Settings")]
    [SerializeField] private int totalWavesRequired = 3;
    
    private int completedWaves = 0;
    private bool victoryConditionMet = false;
    private bool victoryTriggered = false;

    private void Awake()
    {
        if (doorCollider == null)
        {
            doorCollider = GetComponent<BoxCollider2D>();
        }

        if (doorCollider != null)
        {
            doorCollider.isTrigger = false;
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (waveManager == null)
        {
            waveManager = WaveManager.Instance;
        }

        if (waveManager != null)
        {
            waveManager.OnWaveComplete.AddListener(OnWaveCompleted);
            waveManager.OnAllWavesComplete.AddListener(OnAllWavesCompleted);
        }
        else
        {
            Debug.LogError("[VictoryTrigger] WaveManager no encontrado!");
        }
    }

    private void OnWaveCompleted(int waveNumber)
    {
        completedWaves++;
        Debug.Log($"[VictoryTrigger] Wave {waveNumber} completada. Total: {completedWaves}/{totalWavesRequired}");

        CheckVictoryCondition();
    }

    private void OnAllWavesCompleted()
    {
        Debug.Log("[VictoryTrigger] Todas las waves completadas!");
        CheckVictoryCondition();
    }

    private void CheckVictoryCondition()
    {
        if (completedWaves >= totalWavesRequired && !victoryConditionMet)
        {
            victoryConditionMet = true;
            UnlockVictoryDoor();
        }
    }

    private void UnlockVictoryDoor()
    {
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
            Debug.Log("<color=green>[VictoryTrigger] ¡Puerta de victoria desbloqueada! El collider ahora es trigger.</color>");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (victoryConditionMet && !victoryTriggered && other.CompareTag("Player"))
        {
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
        victoryTriggered = true;
        Debug.Log("<color=yellow>=== ¡VICTORIA! ===</color>");

        if (victoryPanel != null)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowVictoryPanel();
            }
            else
            {
                victoryPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        PlayerCombat playerCombat = FindObjectOfType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveComplete.RemoveListener(OnWaveCompleted);
            waveManager.OnAllWavesComplete.RemoveListener(OnAllWavesCompleted);
        }
    }
}
