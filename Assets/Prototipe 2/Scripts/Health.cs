using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityDuration = 1f;

    public int CurrentHealth { get; private set; }
    public bool IsInvulnerable { get; private set; }
    public event Action OnDeath;
    public event Action<int> OnHealthChanged;

    private float invulnerabilityTimer;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Update()
    {
        if (IsInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                IsInvulnerable = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsInvulnerable || CurrentHealth <= 0)
            return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        OnHealthChanged?.Invoke(CurrentHealth);

        IsInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {CurrentHealth}/{maxHealth}");
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}
