using UnityEngine;

/// <summary>
/// Movimiento especializado para el Enemy Flyer.
/// Añade capacidades de hover, seguimiento y mantenimiento de distancia.
/// </summary>
public class FlyerHoverMovement : EnemyMovement
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 3f;
    [SerializeField] private float hoverSpeed = 2f;

    private Vector2 hoverTarget;
    private float hoverTimer;
    private float targetChangeInterval = 3f;

    /// <summary>
    /// Patrulla en el aire con movimiento aleatorio dentro de un área
    /// </summary>
    public void HoverPatrol(Vector2 center, Vector2 areaSize)
    {
        hoverTimer += Time.deltaTime;

        // Cambiar objetivo de hover cada cierto tiempo
        if (hoverTimer >= targetChangeInterval)
        {
            hoverTimer = 0f;

            // Generar nueva posición aleatoria dentro del área
            float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float randomY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

            hoverTarget = center + new Vector2(randomX, hoverHeight + randomY);
        }

        // Moverse hacia objetivo de hover
        float distance = Vector2.Distance(transform.position, hoverTarget);
        if (distance > 0.3f)
        {
            MoveTowards(hoverTarget, hoverSpeed);
        }
        else
        {
            Stop();
        }
    }

    /// <summary>
    /// Mantiene una distancia específica del objetivo
    /// </summary>
    public void MaintainDistance(Vector2 targetPos, float desiredDistance)
    {
        Vector2 currentPos = transform.position;
        float currentDistance = Vector2.Distance(currentPos, targetPos);

        // Calcular dirección hacia/desde el objetivo
        Vector2 direction = (targetPos - currentPos).normalized;

        // Si está muy cerca (< 80% de la distancia deseada) → Alejarse
        if (currentDistance < desiredDistance * 0.8f)
        {
            // Moverse en dirección opuesta
            Vector2 awayPos = currentPos - direction * hoverSpeed * Time.deltaTime;
            MoveTowards(awayPos, hoverSpeed);
        }
        // Si está muy lejos (> 120% de la distancia deseada) → Acercarse
        else if (currentDistance > desiredDistance * 1.2f)
        {
            // Moverse hacia el objetivo
            Vector2 closePos = currentPos + direction * hoverSpeed * Time.deltaTime;
            MoveTowards(closePos, hoverSpeed);
        }
        // Si está a buena distancia → Quedarse quieto (hovering)
        else
        {
            Stop();
        }
    }

    /// <summary>
    /// Moves towards a target while maintaining a specific height
    /// </summary>
    public void MoveTowardsWithHeight(Vector2 target, float speed, float height)
    {
        // Adjust target to maintain height
        Vector2 adjustedTarget = new Vector2(target.x, height);
        MoveTowards(adjustedTarget, speed);
    }

    /// <summary>
    /// Configura los parámetros de hover desde un config
    /// </summary>
    public void SetHoverParameters(float height, float speed)
    {
        hoverHeight = height;
        hoverSpeed = speed;
    }

    /// <summary>
    /// Resetea el timer de hover (útil al cambiar de estado)
    /// </summary>
    public void ResetHoverTimer()
    {
        hoverTimer = 0f;
    }
}
