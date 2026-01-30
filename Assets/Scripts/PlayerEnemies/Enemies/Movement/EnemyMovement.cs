using UnityEngine;

/// <summary>
/// Implementación del sistema de movimiento para enemigos.
/// </summary>
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Variables de patrulla
    private Vector2 patrolPointA;
    private Vector2 patrolPointB;
    private Vector2 currentPatrolTarget;
    private bool movingToB;
    private bool patrolInitialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError($"EnemyMovement on {gameObject.name}: No se encontró Rigidbody2D!");
        }
    }

    /// <summary>
    /// Mueve el enemigo hacia un objetivo específico
    /// CORREGIDO: Solo mueve horizontalmente, respeta gravedad en Y
    /// </summary>
    public void MoveTowards(Vector2 target, float speed)
    {
        if (rb == null) return;

        // Calcular dirección horizontal (izquierda = -1, derecha = 1)
        float directionX = Mathf.Sign(target.x - transform.position.x);

        // Aplicar velocidad SOLO en X, mantener Y para gravedad
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);

        FlipTowards(target);
    }

    /// <summary>
    /// Detiene el movimiento del enemigo
    /// CORREGIDO: Solo detiene movimiento horizontal, mantiene velocidad vertical
    /// </summary>
    public void Stop()
    {
        if (rb == null) return;

        // Solo detener movimiento horizontal, mantener caída/salto
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    /// <summary>
    /// Hace que el enemigo patrulle entre dos puntos
    /// CORREGIDO: Patrulla horizontal, respeta gravedad
    /// </summary>
    public void PatrolBetweenPoints(Vector2 pointA, Vector2 pointB)
    {
        if (rb == null) return;

        // Inicializar puntos de patrulla la primera vez
        if (!patrolInitialized)
        {
            SetPatrolPoints(pointA, pointB);
        }

        // Calcular dirección horizontal hacia el punto objetivo
        float directionX = Mathf.Sign(currentPatrolTarget.x - transform.position.x);

        // Aplicar velocidad solo en X, mantener Y para gravedad
        rb.linearVelocity = new Vector2(directionX * 2f, rb.linearVelocity.y);

        // Voltear hacia el objetivo
        FlipTowards(currentPatrolTarget);

        // Verificar si llegamos al punto objetivo (solo comprobación en X)
        float distanceX = Mathf.Abs(transform.position.x - currentPatrolTarget.x);
        if (distanceX < 0.2f)
        {
            // Cambiar al otro punto
            movingToB = !movingToB;
            currentPatrolTarget = movingToB ? patrolPointB : patrolPointA;
        }
    }

    /// <summary>
    /// Voltea el sprite del enemigo hacia un objetivo
    /// </summary>
    public void FlipTowards(Vector2 target)
    {
        if (spriteRenderer == null) return;

        float direction = target.x - transform.position.x;

        // Si está a la izquierda, flipear
        spriteRenderer.flipX = direction < 0;
    }

    /// <summary>
    /// Configura los puntos de patrulla
    /// </summary>
    public void SetPatrolPoints(Vector2 pointA, Vector2 pointB)
    {
        patrolPointA = pointA;
        patrolPointB = pointB;
        currentPatrolTarget = patrolPointB;
        movingToB = true;
        patrolInitialized = true;
    }

    /// <summary>
    /// Resetea el estado de patrulla
    /// </summary>
    public void ResetPatrol()
    {
        patrolInitialized = false;
        Stop();
    }

    private void OnDrawGizmosSelected()
    {
        if (!patrolInitialized) return;

        // Dibujar línea de patrulla
        Gizmos.color = Color.green;
        Gizmos.DrawLine(patrolPointA, patrolPointB);

        // Dibujar puntos
        Gizmos.DrawWireSphere(patrolPointA, 0.3f);
        Gizmos.DrawWireSphere(patrolPointB, 0.3f);

        // Dibujar objetivo actual
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentPatrolTarget, 0.2f);
    }
}