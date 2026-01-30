using UnityEngine;

/// <summary>
/// Sensor system for detecting the player.
/// Handles range detection, line of sight, and idle detection (for Flyer).
/// UPDATED: Proper obstacle-aware line of sight detection
/// </summary>
public class EnemySensors : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnemyConfigBase config;

    [Header("Line of Sight")]
    [Tooltip("Layers that block line of sight (e.g., Ground, Wall, Platform)")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private Transform player;
    private Vector2 lastPlayerPosition;
    private float idleTimer;

    /// <summary>Current player position</summary>
    public Vector2 PlayerPosition => player != null ? player.position : Vector2.zero;

    /// <summary>Last recorded player position</summary>
    public Vector2 LastPlayerPosition => lastPlayerPosition;

    private void Awake()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"EnemySensors on {gameObject.name}: Player with tag 'Player' not found");
        }

        // Set default obstacle mask if not configured
        if (obstacleMask == 0)
        {
            obstacleMask = LayerMask.GetMask("Ground", "Wall", "Platform");
            Debug.LogWarning($"EnemySensors: obstacleMask not set, using default (Ground, Wall, Platform)");
        }
    }

    private void Start()
    {
        // Get config if not assigned
        if (config == null)
        {
            EnemyBrain brain = GetComponent<EnemyBrain>();
            if (brain != null)
            {
                var context = brain.GetContext();
                if (context != null)
                {
                    config = context.Config;
                }
            }
        }

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Update idle detection
        UpdateIdleDetection();
    }

    /// <summary>
    /// Checks if player is within detection range
    /// </summary>
    public bool PlayerInRange(float range)
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= range;
    }

    /// <summary>
    /// Checks if there's a clear line of sight to player (no obstacles blocking)
    /// UPDATED: Now properly checks for obstacles instead of just player layer
    /// </summary>
    public bool PlayerInLineOfSight()
    {
        if (player == null) return false;

        Vector2 startPos = transform.position;
        Vector2 targetPos = player.position;
        Vector2 direction = targetPos - startPos;
        float distance = direction.magnitude;

        // Raycast to check for obstacles
        RaycastHit2D hit = Physics2D.Raycast(
            startPos,
            direction.normalized,
            distance,
            obstacleMask
        );

        // If raycast hits something → Vision is blocked
        if (hit.collider != null)
        {
            if (showDebugRays)
            {
                Debug.DrawLine(startPos, hit.point, Color.red, 0.1f);
            }
            return false;
        }

        // Clear path to player
        if (showDebugRays)
        {
            Debug.DrawLine(startPos, targetPos, Color.green, 0.1f);
        }
        return true;
    }

    /// <summary>
    /// Unified detection check (range + line of sight)
    /// </summary>
    /// <param name="range">Detection range</param>
    /// <param name="requireLineOfSight">If true, requires clear vision path</param>
    /// <returns>True if player can be detected</returns>
    public bool CanDetectPlayer(float range, bool requireLineOfSight = true)
    {
        if (player == null) return false;

        // Check distance first (cheaper operation)
        if (!PlayerInRange(range))
            return false;

        // Then check line of sight if required
        if (requireLineOfSight && !PlayerInLineOfSight())
            return false;

        return true;
    }

    /// <summary>
    /// Updates idle detection timer
    /// </summary>
    private void UpdateIdleDetection()
    {
        Vector2 currentPos = player.position;
        float movement = Vector2.Distance(currentPos, lastPlayerPosition);

        // Only for FlyerEnemyConfig
        if (config is FlyerEnemyConfig flyerConfig)
        {
            // If movement is less than threshold, increment timer
            if (movement < flyerConfig.idlePositionThreshold)
            {
                idleTimer += Time.deltaTime;
            }
            else
            {
                // Player is moving, reset timer
                idleTimer = 0f;
                lastPlayerPosition = currentPos;
            }
        }
    }

    /// <summary>
    /// Checks if player is idle (for Flyer detection)
    /// </summary>
    public bool PlayerIsIdle()
    {
        if (config is FlyerEnemyConfig flyerConfig)
        {
            return idleTimer >= flyerConfig.idleDetectionTime;
        }

        return false;
    }

    /// <summary>
    /// Resets idle detection timer
    /// </summary>
    public void ResetIdleTimer()
    {
        idleTimer = 0f;
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null || config == null) return;

        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.position;

        // Draw line to player (green if clear, red if blocked)
        bool hasLOS = PlayerInLineOfSight();
        Gizmos.color = hasLOS ? Color.green : Color.red;
        Gizmos.DrawLine(enemyPos, playerPos);

        // Draw detection range circle
        Gizmos.color = Color.yellow;
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.1f);
        UnityEditor.Handles.DrawSolidDisc(enemyPos, Vector3.forward, config.detectionRange);

        Gizmos.color = Color.yellow;
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(enemyPos, Vector3.forward, config.detectionRange);
#endif
    }
}