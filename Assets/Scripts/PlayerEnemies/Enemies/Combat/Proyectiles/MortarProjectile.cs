using UnityEngine;

/// <summary>
/// Physics-based mortar projectile that travels in an arc.
/// Uses Rigidbody2D for realistic collisions and trajectory.
/// Used by Enemy Shooter.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MortarProjectile : MonoBehaviour
{
    [Header("Impact Effects")]
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private bool showDebugLogs = false;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private int damage;
    private float lifetime;
    private bool initialized = false;

    // Visual helpers
    private Vector2 startPosition;
    private float arcHeight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("MortarProjectile: Rigidbody2D component missing!");
        }
    }

    /// <summary>
    /// Initializes the projectile with physics-based trajectory
    /// </summary>
    /// <param name="target">Target position to aim at</param>
    /// <param name="projectileSpeed">Desired speed (used to calculate flight time)</param>
    /// <param name="arc">Desired arc height</param>
    /// <param name="dmg">Damage on hit</param>
    /// <param name="life">Lifetime before auto-destruction</param>
    public void Initialize(Vector2 target, float projectileSpeed, float arc, int dmg, float life)
    {
        if (rb == null)
        {
            Debug.LogError("MortarProjectile: Cannot initialize without Rigidbody2D!");
            return;
        }

        targetPosition = target;
        arcHeight = arc;
        damage = dmg;
        lifetime = life;
        startPosition = transform.position;

        // Calculate initial velocity for arc trajectory
        Vector2 velocity = CalculateArcVelocity(startPosition, targetPosition, arcHeight);
        rb.linearVelocity = velocity;

        initialized = true;

        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[MortarProjectile]</color> Launched with velocity {velocity} towards {target}");
        }
    }

    /// <summary>
    /// Calculates initial velocity needed to hit target with desired arc
    /// Uses projectile motion physics equations
    /// </summary>
    private Vector2 CalculateArcVelocity(Vector2 start, Vector2 target, float desiredHeight)
    {
        // Get gravity magnitude (positive value)
        float gravity = Mathf.Abs(Physics2D.gravity.y);

        // If no gravity, use default
        if (gravity < 0.1f)
        {
            gravity = 9.81f;
            Debug.LogWarning("MortarProjectile: Physics2D.gravity is too low, using default 9.81");
        }

        Vector2 displacement = target - start;
        float horizontalDistance = displacement.x;
        float verticalDistance = displacement.y;

        // Calculate time to reach peak height
        float timeToApex = Mathf.Sqrt(2f * desiredHeight / gravity);

        // Calculate time to fall from apex to target height
        float heightDifference = desiredHeight - verticalDistance;
        float timeFromApex = Mathf.Sqrt(2f * Mathf.Max(0, heightDifference) / gravity);

        // Total flight time
        float flightTime = timeToApex + timeFromApex;

        // Avoid division by zero
        if (flightTime < 0.1f) flightTime = 0.1f;

        // Calculate velocities
        float velocityX = horizontalDistance / flightTime;
        float velocityY = Mathf.Sqrt(2f * gravity * desiredHeight);

        return new Vector2(velocityX, velocityY);
    }

    private void Update()
    {
        if (!initialized) return;

        // Rotate projectile to face movement direction
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!initialized) return;

        Transform root = collision.transform.root;

        // Damage player
        if (root.CompareTag("Player"))
        {
            IDamageable playerHealth = root.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                if (showDebugLogs)
                {
                    Debug.Log($"<color=red>[MortarProjectile]</color> Hit player! Dealt {damage} damage");
                }
            }

            OnImpact(collision.ClosestPoint(transform.position));
            return;
        }

        // Destroy on collision with ground/walls
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>[MortarProjectile]</color> Hit {collision.gameObject.name}");
            }

            OnImpact(collision.ClosestPoint(transform.position));
        }
    }

    /// <summary>
    /// Called when projectile impacts something
    /// </summary>
    private void OnImpact(Vector2 impactPoint)
    {
        // Spawn impact VFX if assigned
        if (impactVFXPrefab != null)
        {
            Instantiate(impactVFXPrefab, impactPoint, Quaternion.identity);
        }

        // Destroy projectile
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!initialized) return;

        // Draw predicted trajectory arc
        Gizmos.color = Color.cyan;

        Vector2 lastPos = startPosition;
        int segments = 20;
        float timeStep = 0.1f;

        Vector2 currentVelocity = rb != null ? rb.linearVelocity : Vector2.zero;
        float gravity = Mathf.Abs(Physics2D.gravity.y);

        for (int i = 1; i <= segments; i++)
        {
            float time = i * timeStep;

            // Projectile motion equation: position = initialPos + velocity*t + 0.5*gravity*t�
            Vector2 pos = startPosition;
            pos.x += currentVelocity.x * time;
            pos.y += currentVelocity.y * time - 0.5f * gravity * time * time;

            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;

            // Stop drawing if trajectory goes too far
            if (Vector2.Distance(startPosition, pos) > 20f) break;
        }

        // Draw target position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);
    }
}