using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    [Header("Magic Settings")]
    [SerializeField] private float magicCooldown = 3f;
    
    [Header("Available Spells")]
    [SerializeField] private List<MagicSpellData> availableSpells = new List<MagicSpellData>();
    [SerializeField] private int currentSpellIndex = 0;
    
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
        
        if (availableSpells.Count == 0)
        {
            Debug.LogWarning("MagicSystem: No hay magias configuradas! Añade MagicSpellData assets en el Inspector.");
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0 && !QTEManager.Instance.IsQTEActive())
        {
            if (InputManager.Magic1WasPressed && availableSpells.Count > 0)
            {
                SetSpell(0);
                CastMagic();
            }
            else if (InputManager.Magic2WasPressed && availableSpells.Count > 1)
            {
                SetSpell(1);
                CastMagic();
            }
            else if (InputManager.Magic3WasPressed && availableSpells.Count > 2)
            {
                SetSpell(2);
                CastMagic();
            }
        }
    }

    private void CastMagic()
    {
        if (availableSpells.Count == 0)
        {
            Debug.LogWarning("No hay magias disponibles para lanzar!");
            return;
        }
        
        MagicSpellData spell = availableSpells[currentSpellIndex];
        
        if (spell.buttons.Count == 0)
        {
            Debug.LogWarning($"La magia '{spell.spellName}' no tiene botones configurados!");
            return;
        }
        
        Debug.Log($"Lanzando: {spell.spellName}");
        
        List<QTEButton> buttons = new List<QTEButton>();
        foreach (var buttonData in spell.buttons)
        {
            buttons.Add(new QTEButton(buttonData.key, buttonData.position));
        }
        
        QTEManager.Instance.StartQTE(buttons);
        
        cooldownTimer = magicCooldown * spell.cooldownMultiplier;
    }

    private void OnQTECompleted(int score)
    {
        if (availableSpells.Count == 0) return;
        
        MagicSpellData spell = availableSpells[currentSpellIndex];
        int maxScore = spell.buttons.Count * 3;
        
        if (score >= maxScore)
        {
            Debug.Log($"=== {spell.spellName.ToUpper()} PERFECTA === ({score}/{maxScore} puntos)");
        }
        else if (score >= maxScore / 2)
        {
            Debug.Log($"=== {spell.spellName.ToUpper()} PARCIAL === ({score}/{maxScore} puntos, 50% efectividad)");
        }
        else
        {
            Debug.Log($"=== {spell.spellName.ToUpper()} FALLIDA === ({score}/{maxScore} puntos)");
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
    
    public void SetSpell(int index)
    {
        if (index >= 0 && index < availableSpells.Count)
        {
            currentSpellIndex = index;
            Debug.Log($"Magia seleccionada: {availableSpells[currentSpellIndex].spellName}");
        }
        else
        {
            Debug.LogWarning($"Índice de magia inválido: {index}");
        }
    }
    
    public void NextSpell()
    {
        if (availableSpells.Count == 0) return;
        
        currentSpellIndex = (currentSpellIndex + 1) % availableSpells.Count;
        Debug.Log($"Magia seleccionada: {availableSpells[currentSpellIndex].spellName}");
    }
    
    public void PreviousSpell()
    {
        if (availableSpells.Count == 0) return;
        
        currentSpellIndex--;
        if (currentSpellIndex < 0)
            currentSpellIndex = availableSpells.Count - 1;
        
        Debug.Log($"Magia seleccionada: {availableSpells[currentSpellIndex].spellName}");
    }
    
    public string GetCurrentSpellName()
    {
        if (availableSpells.Count == 0 || currentSpellIndex >= availableSpells.Count)
            return "Sin magia";
        
        return availableSpells[currentSpellIndex].spellName;
    }
}
