using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    [Header("Magic Settings")]
    [SerializeField] private float magicCooldown = 3f;
    
    private float cooldownTimer;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.OnQTEComplete += OnQTECompleted;
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (InputManager.MagicWasPressed && cooldownTimer <= 0 && !QTEManager.Instance.IsQTEActive())
        {
            CastMagic();
        }
    }

    private void CastMagic()
    {
        Debug.Log("Magic cast initiated!");
        
        List<QTEButton> buttons = GenerateRandomButtons();
        QTEManager.Instance.StartQTE(buttons);
        
        cooldownTimer = magicCooldown;
    }

    private List<QTEButton> GenerateRandomButtons()
    {
        List<QTEButton> buttons = new List<QTEButton>();
        KeyCode[] availableKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
        
        for (int i = 0; i < 3; i++)
        {
            float thirdStart = i / 3f;
            float thirdEnd = (i + 1) / 3f;
            float centerOffset = 1f / 6f;
            float randomRange = 0.1f;
            
            float targetPos = thirdStart + centerOffset + Random.Range(-randomRange, randomRange);
            targetPos = Mathf.Clamp(targetPos, thirdStart + 0.05f, thirdEnd - 0.05f);
            
            KeyCode randomKey = availableKeys[Random.Range(0, availableKeys.Length)];
            buttons.Add(new QTEButton(randomKey, targetPos));
        }
        
        return buttons;
    }

    private void OnQTECompleted(int score)
    {
        if (score >= 9)
        {
            Debug.Log("=== MAGIA PERFECTA ===");
        }
        else if (score >= 4)
        {
            Debug.Log("=== MAGIA PARCIAL (50% efectividad) ===");
        }
        else
        {
            Debug.Log("=== MAGIA FALLIDA ===");
        }
    }

    private void OnDestroy()
    {
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.OnQTEComplete -= OnQTECompleted;
        }
    }

    public float GetCooldownProgress()
    {
        return Mathf.Clamp01(1f - (cooldownTimer / magicCooldown));
    }
}
