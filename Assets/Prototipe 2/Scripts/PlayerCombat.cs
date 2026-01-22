using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;

    private Health health;
    private PlayerMovement playerMovement;

    private float attackTimer;

    private void Awake()
    {
        health = GetComponent<Health>();
        playerMovement = GetComponent<PlayerMovement>();

        health.OnDeath += HandleDeath;
    }

    private void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (InputManager.AttackWasPressed && attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    private void PerformAttack()
    {
        float attackDirection = playerMovement.IsFacingRight ? 1f : -1f;
        Vector2 attackPosition = (Vector2)transform.position + (Vector2.right * attackDirection * attackRange);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, 0.5f, enemyLayer);

        Debug.Log($"Player attacked in direction {(playerMovement.IsFacingRight ? "right" : "left")}! Position: {attackPosition}");

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackDirection = new Vector2(attackDirection, 0.5f).normalized;
                    enemyRb.linearVelocity = knockbackDirection * knockbackForce;
                }
                
                Debug.Log($"Player attacked {hit.name}!");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(1);

            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
            
        float attackDirection = playerMovement != null && playerMovement.IsFacingRight ? 1f : -1f;
        Vector2 attackPosition = (Vector2)transform.position + (Vector2.right * attackDirection * attackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, 0.5f);
    }
}
