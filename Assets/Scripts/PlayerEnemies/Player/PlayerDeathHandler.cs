using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        // Obtener referencia si no está asignada
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        // Validate we have the necessary reference
        if (playerHealth == null)
        {
            Debug.LogError("PlayerDeathHandler: PlayerHealth not found on Player!");
            return;
        }

        // Subscribe ONLY to the HealthComponent death event
        playerHealth.OnDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    /// <summary>
    /// Called when the player dies (CurrentHealth = 0)
    /// Now activates Game Over directly, without respawn
    /// </summary>
    private void HandlePlayerDeath()
    {
        Debug.Log("=== GAME OVER - PLAYER DIED ===");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null! Cannot show Game Over Panel");
        }
    }
}