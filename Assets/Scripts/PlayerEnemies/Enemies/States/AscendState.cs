using UnityEngine;

/// <summary>
/// Ascent state for Flyer - Returns to safe altitude after diving.
/// Flyer is vulnerable but moves quickly upward.
/// </summary>
public class AscendState : IEnemyState
{
    private EnemyContext context;
    private EnemyBrain brain;
    private FlyerEnemyConfig flyerConfig;

    private float targetHeight;
    private float startHeight;
    private float elapsedTime;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public AscendState(EnemyContext context, EnemyBrain brain)
    {
        this.context = context;
        this.brain = brain;
        this.flyerConfig = context.Config as FlyerEnemyConfig;
    }

    public void Enter()
    {
        Debug.Log($"<color=cyan>[{context.Enemy.name}]</color> ⬆️ Entering <b>ASCEND</b> (rising to safe altitude)");

        // Verify this is a Flyer
        if (flyerConfig == null)
        {
            Debug.LogError($"AscendState: Config is not FlyerEnemyConfig on {context.Enemy.name}!");
            brain.ChangeState(new PatrolState(context, brain));
            return;
        }

        // Calculate target height (hybrid approach)
        float playerBasedHeight = context.Player != null
            ? context.Player.position.y + flyerConfig.playerHeightOffset
            : flyerConfig.minimumPatrolHeight;

        targetHeight = Mathf.Max(playerBasedHeight, flyerConfig.minimumPatrolHeight);
        startHeight = context.Enemy.position.y;
        elapsedTime = 0f;

        // Get sprite for visual feedback
        spriteRenderer = context.Enemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            // Keep normal color (or slight tint if you want)
            // For now, just white/original color
            spriteRenderer.color = originalColor;
        }

        float distanceToAscend = targetHeight - startHeight;
        Debug.Log($"<color=cyan>[AscendState]</color> Target height: {targetHeight:F1}m, Distance to ascend: {distanceToAscend:F1}m");

        // Stop horizontal movement during ascent
        if (context.Movement != null)
        {
            context.Movement.Stop();
        }
    }

    public void Tick(float deltaTime)
    {
        if (flyerConfig == null) return;

        elapsedTime += deltaTime;

        // Get current position
        Vector2 currentPos = context.Enemy.position;
        float currentHeight = currentPos.y;

        // Move upward using MoveTowards (constant speed)
        float newHeight = Mathf.MoveTowards(
            currentHeight,
            targetHeight,
            flyerConfig.ascentSpeed * deltaTime
        );

        // Apply new position
        context.Enemy.position = new Vector2(currentPos.x, newHeight);

        // Check completion conditions
        float distanceRemaining = Mathf.Abs(targetHeight - newHeight);

        // Transition to Patrol when:
        // 1. Reached target height (within threshold)
        // 2. OR timeout (safety)
        if (distanceRemaining <= flyerConfig.ascentCompleteThreshold ||
            elapsedTime >= flyerConfig.maxAscentDuration)
        {
            if (elapsedTime >= flyerConfig.maxAscentDuration)
            {
                Debug.LogWarning($"<color=yellow>[AscendState]</color> Timeout reached, transitioning anyway");
            }
            else
            {
                Debug.Log($"<color=cyan>[AscendState]</color> Reached target height, transitioning to Patrol");
            }

            brain.ChangeState(new PatrolState(context, brain));
        }
    }

    public void Exit()
    {
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        Debug.Log($"<color=cyan>[{context.Enemy.name}]</color> Exiting ASCEND state");
    }
}
