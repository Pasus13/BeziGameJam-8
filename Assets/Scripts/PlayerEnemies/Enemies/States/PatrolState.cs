using UnityEngine;

/// <summary>
/// Patrol state - Enemy patrols between two points.
/// Transitions to PrepareAttackState when detecting player.
/// VERSION WITH ADAPTIVE PATROL RECALCULATION
/// </summary>
public class PatrolState : IEnemyState
{
    private EnemyContext context;
    private EnemyBrain brain;
    private Vector2 patrolPointA;
    private Vector2 patrolPointB;
    private float patrolSpeed;

    // Visual feedback variables
    private bool wasPlayerInRange = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Adaptive patrol recalculation
    private Vector2 lastPatrolCalculationPosition;
    private const float MIN_DISTANCE_FOR_RECALCULATION = 5f;

    public PatrolState(EnemyContext context, EnemyBrain brain)
    {
        this.context = context;
        this.brain = brain;
    }

    public void Enter()
    {
        Debug.Log($"<color=green>[{context.Enemy.name}]</color> 🚶 Entering <b>PATROL</b>");

        // Verify config exists
        if (context.Config == null)
        {
            Debug.LogError($"PatrolState: Config is NULL on {context.Enemy.name}!");
            return;
        }

        // Get sprite renderer for visual feedback
        spriteRenderer = context.Enemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Calculate initial patrol
        SetupPatrolPointsAdaptive();

        // Store position where patrol was calculated
        lastPatrolCalculationPosition = context.Enemy.position;

        patrolSpeed = context.Config.patrolSpeed;
    }

    public void Tick(float deltaTime)
    {
        // Verify we have everything needed
        if (context.Config == null || context.Movement == null) return;

        // Patrol between points
        context.Movement.PatrolBetweenPoints(patrolPointA, patrolPointB);

        // Unified detection (range + LOS)
        bool playerInRange = context.Sensors != null &&
                            context.Sensors.CanDetectPlayer(context.Config.detectionRange, requireLineOfSight: true);

        // Visual feedback: Change color based on detection
        if (spriteRenderer != null)
        {
            spriteRenderer.color = playerInRange ? Color.red : originalColor;
        }

        // NEW: Recalculate patrol when losing player
        if (!playerInRange && wasPlayerInRange)
        {
            Debug.Log($"<color=cyan>[{context.Enemy.name}]</color> ℹ️ Player left range - Recalculating patrol");
            RecalculatePatrol();
        }

        // NEW: Recalculate if moved significantly
        float distanceMoved = Vector2.Distance(context.Enemy.position, lastPatrolCalculationPosition);
        if (distanceMoved > MIN_DISTANCE_FOR_RECALCULATION)
        {
            Debug.Log($"<color=yellow>[{context.Enemy.name}]</color> Moved {distanceMoved:F1}m - Recalculating patrol");
            RecalculatePatrol();
        }

        // Log when entering/exiting range
        if (playerInRange && !wasPlayerInRange)
        {
            Debug.Log($"<color=yellow>[{context.Enemy.name}]</color> ⚠️ PLAYER DETECTED IN RANGE!");
        }

        wasPlayerInRange = playerInRange;

        // For Flyer: Transition to AwareState first
        if (ShouldTransitionToAware())
        {
            brain.ChangeState(new AwareState(context, brain));
            return;
        }

        // For other enemies: Direct transition to PrepareAttackState
        if (ShouldTransitionToAttack())
        {
            brain.ChangeState(new PrepareAttackState(context, brain));
        }
    }

    public void Exit()
    {
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (context.Movement != null)
        {
            context.Movement.Stop();
        }
    }

    /// <summary>
    /// Sets up patrol points adaptively using PlatformDetector
    /// </summary>
    private void SetupPatrolPointsAdaptive()
    {
        // Try to use platform detection
        PlatformDetector detector = context.Enemy.GetComponent<PlatformDetector>();

        if (detector != null)
        {
            // Use intelligent detection
            var (pointA, pointB) = detector.DetectPatrolPoints(
                context.Config,
                minPatrolWidth: 2f
            );

            patrolPointA = pointA;
            patrolPointB = pointB;

            Debug.Log($"<color=cyan>[PatrolState]</color> Adaptive patrol: {pointA} → {pointB} ({Vector2.Distance(pointA, pointB):F1}m)");
        }
        else
        {
            // Fallback: Old system based on spawn
            Debug.LogWarning($"<color=yellow>[PatrolState]</color> No PlatformDetector, using spawn-based patrol");
            SetupPatrolPoints();
        }
    }

    /// <summary>
    /// Recalculates patrol points from current position
    /// Called when losing player or after significant movement
    /// </summary>
    private void RecalculatePatrol()
    {
        // Recalculate patrol points
        SetupPatrolPointsAdaptive();

        // Update stored position
        lastPatrolCalculationPosition = context.Enemy.position;

        // Reset movement state to avoid weird transitions
        if (context.Movement != null)
        {
            context.Movement.ResetPatrol();
        }
    }

    /// <summary>
    /// Sets up patrol points based on enemy type (FALLBACK)
    /// </summary>
    private void SetupPatrolPoints()
    {
        if (context.Config == null) return;

        float patrolDistance = 3f; // Default value

        // Get patrol distance based on config type
        if (context.Config is ChargeEnemyConfig charge)
        {
            patrolDistance = charge.patrolDistance;
        }
        else if (context.Config is ShooterEnemyConfig shooter)
        {
            patrolDistance = shooter.patrolDistance;
        }
        else if (context.Config is FlyerEnemyConfig flyer)
        {
            patrolDistance = flyer.patrolAreaSize.x;
        }

        // Calculate patrol points from spawn position
        patrolPointA = context.SpawnPosition + Vector2.left * patrolDistance;
        patrolPointB = context.SpawnPosition + Vector2.right * patrolDistance;
    }

    /// <summary>
    /// Checks if should transition to AwareState (Flyer only)
    /// </summary>
    private bool ShouldTransitionToAware()
    {
        // Only for Flyer
        if (!(context.Config is FlyerEnemyConfig)) return false;

        // Must be in range
        if (!context.Sensors.PlayerInRange(context.Config.detectionRange))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if should transition to attack state
    /// </summary>
    private bool ShouldTransitionToAttack()
    {
        // Verify we have everything needed
        if (context.Config == null || context.Sensors == null) return false;

        // 1. Must be in range AND have line of sight
        if (!context.Sensors.CanDetectPlayer(context.Config.detectionRange, requireLineOfSight: true))
            return false;

        // 2. Cooldown must be ready
        if (context.AttackCooldown == null || !context.AttackCooldown.IsReady)
            return false;

        // 3. Attack must be able to execute
        if (context.Attack == null || !context.Attack.CanStartAttack())
            return false;

        return true;
    }
}