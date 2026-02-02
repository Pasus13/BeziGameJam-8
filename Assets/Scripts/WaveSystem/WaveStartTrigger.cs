using UnityEngine;

/// <summary>
/// Trigger that starts the wave system when player presses Enter
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WaveStartTrigger : MonoBehaviour
{
    [Header("Prompt Settings")]
    [SerializeField] private string promptMessage = "Pulsa Enter para comenzar los sacrificios";
    [SerializeField] private KeyCode activationKey = KeyCode.Return;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private bool showParticlesOnlyWhenNear = true;

    [Header("Audio")]
    [SerializeField] private AudioClip activationSound;

    private bool playerInRange = false;
    private bool hasBeenActivated = false;
    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        // Setup particles
        if (particles != null)
        {
            if (showParticlesOnlyWhenNear)
            {
                particles.Stop();
            }
            else
            {
                particles.Play();
            }
        }
    }

    private void Update()
    {
        // Check for activation input
        if (playerInRange && !hasBeenActivated)
        {
            if (Input.GetKeyDown(activationKey))
            {
                ActivateTrigger();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenActivated) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            // Show particles
            if (particles != null && showParticlesOnlyWhenNear)
            {
                particles.Play();
            }

            // Show prompt
            if (TriggerPromptUI.Instance != null)
            {
                TriggerPromptUI.Instance.ShowPrompt(promptMessage);
            }

            Debug.Log("<color=cyan>[WaveStartTrigger]</color> Player entered trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hasBeenActivated) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide particles
            if (particles != null && showParticlesOnlyWhenNear)
            {
                particles.Stop();
            }

            // Hide prompt
            if (TriggerPromptUI.Instance != null)
            {
                TriggerPromptUI.Instance.HidePrompt();
            }

            Debug.Log("<color=yellow>[WaveStartTrigger]</color> Player exited trigger");
        }
    }

    private void ActivateTrigger()
    {
        hasBeenActivated = true;
        playerInRange = false;

        Debug.Log("<color=green>[WaveStartTrigger]</color> ========== ACTIVATING WAVES ==========");

        // Hide prompt
        if (TriggerPromptUI.Instance != null)
        {
            TriggerPromptUI.Instance.HidePrompt();
        }

        // Play activation sound
        if (activationSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.SFX.PlaySFX(activationSound);
        }

        // Stop particles
        if (particles != null)
        {
            particles.Stop();
        }

        // Start battle music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBattleMusic();
        }

        // Start waves
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.StartWavesFromTrigger();
        }
        else
        {
            Debug.LogError("<color=red>[WaveStartTrigger]</color> WaveManager.Instance is NULL!");
        }

        // Disable trigger
        triggerCollider.enabled = false;

        Debug.Log("<color=green>[WaveStartTrigger]</color> Trigger activated and disabled");
    }

    private void OnDrawGizmos()
    {
        // Visualize trigger area in editor
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = hasBeenActivated ? Color.gray : Color.cyan;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}
