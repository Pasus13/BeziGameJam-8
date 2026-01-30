using UnityEngine;

/// <summary>
/// Estado de "Aware" para el Enemy Flyer.
/// El Flyer persigue al jugador manteniendo una distancia segura.
/// Transiciona a PrepareAttackState cuando el jugador está idle.
/// </summary>
public class AwareState : IEnemyState
{
    private EnemyContext context;
    private EnemyBrain brain;
    private FlyerEnemyConfig flyerConfig;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector2 lastTrackedPlayerPosition;
    private bool hasTrackedPosition = false;

    public AwareState(EnemyContext context, EnemyBrain brain)
    {
        this.context = context;
        this.brain = brain;
        flyerConfig = context.Config as FlyerEnemyConfig;
    }

    public void Enter()
    {
        Debug.Log($"<color=orange>[{context.Enemy.name}]</color> 🎯 Entrando en <b>AWARE</b> (persiguiendo al jugador)");

        // Verificar que es un Flyer
        if (flyerConfig == null)
        {
            Debug.LogError($"AwareState: Config no es FlyerEnemyConfig en {context.Enemy.name}!");
            brain.ChangeState(new PatrolState(context, brain));
            return;
        }

        // Obtener sprite para feedback visual
        spriteRenderer = context.Enemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red; // Rojo mientras persigue
        }

        // Resetear timer de idle
        if (context.Sensors != null)
        {
            context.Sensors.ResetIdleTimer();
        }

        if (context.Player != null)
        {
            lastTrackedPlayerPosition = context.Player.position;
            hasTrackedPosition = true;
        }
    }

    public void Tick(float deltaTime)
    {
        if (flyerConfig == null || context.Player == null)
        {
            brain.ChangeState(new PatrolState(context, brain));
            return;
        }

        Vector2 playerPos = context.Player.position;
        Vector2 flyerPos = context.Enemy.position;
        float currentDistance = Vector2.Distance(flyerPos, playerPos);

        // Apply deadzone - only adjust if player moved significantly
        if (PlayerMovedSignificantly())
        {
            // Maintain distance using the movement system
            FlyerHoverMovement flyerMovement = context.Movement as FlyerHoverMovement;
            if (flyerMovement != null)
            {
                // Use specialized Flyer movement
                flyerMovement.MaintainDistance(playerPos, flyerConfig.followDistance);
            }
            else
            {
                // Fallback to basic movement
                MaintainDistanceBasic(playerPos, flyerConfig.followDistance);
            }

            // Update tracked position after adjustment
            UpdateTrackedPosition();
        }

        // Always flip towards player (even if not moving)
        context.Movement.FlipTowards(playerPos);

        // Transition 1: If player is idle → PrepareAttackState
        if (context.Sensors != null && context.Sensors.PlayerIsIdle())
        {
            if (context.AttackCooldown != null && context.AttackCooldown.IsReady)
            {
                if (context.Attack != null && context.Attack.CanStartAttack())
                {
                    Debug.Log($"<color=yellow>[{context.Enemy.name}]</color> Player idle detected! Preparing attack...");
                    brain.ChangeState(new PrepareAttackState(context, brain));
                    return;
                }
            }
        }

        // Transition 2: If player exits range → PatrolState
        if (!context.Sensors.PlayerInRange(context.Config.detectionRange * 1.5f))
        {
            Debug.Log($"<color=cyan>[{context.Enemy.name}]</color> Player out of range, returning to patrol");
            brain.ChangeState(new PatrolState(context, brain));
        }
    }

    public void Exit()
    {
        // Restaurar color original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Detener movimiento
        if (context.Movement != null)
        {
            context.Movement.Stop();
        }

        // Resetear timer de idle
        if (context.Sensors != null)
        {
            context.Sensors.ResetIdleTimer();
        }
    }

    /// <summary>
    /// Mantiene distancia usando movimiento básico (fallback)
    /// </summary>
    private void MaintainDistanceBasic(Vector2 targetPos, float desiredDistance)
    {
        Vector2 flyerPos = context.Enemy.position;
        Vector2 direction = (targetPos - flyerPos).normalized;
        float currentDistance = Vector2.Distance(flyerPos, targetPos);

        // Muy cerca → Alejarse
        if (currentDistance < desiredDistance * 0.8f)
        {
            Vector2 awayTarget = flyerPos - direction * 2f;
            context.Movement.MoveTowards(awayTarget, flyerConfig.hoverSpeed);
        }
        // Muy lejos → Acercarse
        else if (currentDistance > desiredDistance * 1.2f)
        {
            Vector2 closeTarget = flyerPos + direction * 2f;
            context.Movement.MoveTowards(closeTarget, flyerConfig.hoverSpeed);
        }
        // A buena distancia → Quedarse quieto
        else
        {
            context.Movement.Stop();
        }
    }

    /// <summary>
    /// Checks if player has moved enough to warrant position adjustment
    /// </summary>
    private bool PlayerMovedSignificantly()
    {
        if (!hasTrackedPosition) return true;

        Vector2 currentPos = context.Player.position;
        float deltaX = Mathf.Abs(currentPos.x - lastTrackedPlayerPosition.x);
        float deltaY = Mathf.Abs(currentPos.y - lastTrackedPlayerPosition.y);

        return deltaX >= flyerConfig.awarenessDeadzoneX ||
               deltaY >= flyerConfig.awarenessDeadzoneY;
    }

    /// <summary>
    /// Updates the tracked player position (call after adjusting Flyer position)
    /// </summary>
    private void UpdateTrackedPosition()
    {
        if (context.Player != null)
        {
            lastTrackedPlayerPosition = context.Player.position;
            hasTrackedPosition = true;
        }
    }
}
