using UnityEngine;

/// <summary>
/// Detecta la plataforma actual del enemigo y calcula puntos de patrulla inteligentes.
/// Limita patrullas grandes seg�n maxPatrolWidth del config.
/// </summary>
public class PlatformDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Layer donde est�n el suelo y plataformas")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Distancia m�xima de raycast lateral")]
    [SerializeField] private float maxRaycastDistance = 50f;

    [Tooltip("Distancia de detecci�n de suelo debajo")]
    [SerializeField] private float groundCheckDistance = 1f;

    [Tooltip("Incremento de cada raycast (menor = m�s preciso pero m�s lento)")]
    [SerializeField] private float raycastStep = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    [SerializeField] private bool showGizmos = true;

    // Variables de debug
    private Vector2 lastDetectedPointA;
    private Vector2 lastDetectedPointB;
    private bool hasDetectedPoints;

    /// <summary>
    /// Detecta la plataforma actual y retorna puntos de patrulla v�lidos
    /// </summary>
    /// <param name="config">Configuraci�n del enemigo (para obtener maxPatrolWidth)</param>
    /// <param name="minPatrolWidth">Ancho m�nimo de patrulla</param>
    /// <returns>Tupla con punto A y punto B de patrulla</returns>
    public (Vector2 pointA, Vector2 pointB) DetectPatrolPoints(
        EnemyConfigBase config,
        float minPatrolWidth = 2f)
    {
        Vector2 currentPos = transform.position;

        // 1. Detectar bordes reales de la plataforma
        float leftEdge = FindPlatformEdge(currentPos, Vector2.left);
        float rightEdge = FindPlatformEdge(currentPos, Vector2.right);
        float platformWidth = rightEdge - leftEdge;

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[PlatformDetector]</color> Plataforma detectada: {platformWidth:F1}m de ancho");
        }

        // 2. LIMITAR si plataforma muy grande (CLAVE PARA SUELO COMPLETO)
        float maxWidth = config.maxPatrolWidth;
        if (platformWidth > maxWidth)
        {
            // Centrar patrulla en posici�n actual del enemigo
            float halfWidth = maxWidth / 2f;
            leftEdge = currentPos.x - halfWidth;
            rightEdge = currentPos.x + halfWidth;

            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>[PlatformDetector]</color> Plataforma grande ({platformWidth:F1}m) limitada a {maxWidth:F1}m");
            }
        }

        // 3. Aplicar l�mites de arena (nunca exceder bounds)
        if (ArenaLimits.Instance != null)
        {
            Bounds arenaBounds = ArenaLimits.Instance.GetBounds();
            leftEdge = Mathf.Max(leftEdge, arenaBounds.min.x + 0.5f);
            rightEdge = Mathf.Min(rightEdge, arenaBounds.max.x - 0.5f);
        }

        // 4. Verificar ancho m�nimo
        float finalWidth = rightEdge - leftEdge;
        if (finalWidth < minPatrolWidth)
        {
            float center = (leftEdge + rightEdge) / 2f;
            leftEdge = center - minPatrolWidth / 2f;
            rightEdge = center + minPatrolWidth / 2f;

            if (showDebugLogs)
            {
                Debug.Log($"<color=orange>[PlatformDetector]</color> Patrulla muy peque�a, ampliada a {minPatrolWidth}m");
            }
        }

        // 5. Find Y height (ground below the enemy)
        float groundY = FindGroundY(currentPos);

        // Save for Gizmos
        lastDetectedPointA = new Vector2(leftEdge, groundY);
        lastDetectedPointB = new Vector2(rightEdge, groundY);
        hasDetectedPoints = true;

        return (lastDetectedPointA, lastDetectedPointB);
    }

    /// <summary>
    /// Encuentra el borde de la plataforma en una direcci�n
    /// </summary>
    private float FindPlatformEdge(Vector2 startPos, Vector2 direction)
    {
        float checkY = startPos.y - groundCheckDistance;

        for (float distance = raycastStep; distance < maxRaycastDistance; distance += raycastStep)
        {
            Vector2 checkPos = startPos + direction * distance;
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance * 2f, groundLayer);

            // Si no hay suelo, encontramos el borde
            if (!hit.collider)
            {
                // Retornar un poco antes del borde para seguridad
                return checkPos.x - direction.x * 0.5f;
            }
        }

        // Si llegamos al m�ximo, usar esa posici�n
        return startPos.x + direction.x * maxRaycastDistance;
    }

    /// <summary>
    /// Finds the Y height of the ground below the enemy
    /// </summary>
    private float FindGroundY(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 10f, groundLayer);

        if (hit.collider)
        {
            // A bit above ground so it doesn't sink
            return hit.point.y + 0.5f;
        }

        // If no ground found, keep current height
        return pos.y;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || !hasDetectedPoints) return;

        // Dibujar l�nea de patrulla detectada
        Gizmos.color = Color.green;
        Gizmos.DrawLine(lastDetectedPointA, lastDetectedPointB);

        // Dibujar puntos
        Gizmos.DrawWireSphere(lastDetectedPointA, 0.3f);
        Gizmos.DrawWireSphere(lastDetectedPointB, 0.3f);

        // Dibujar label con distancia
        Vector2 center = (lastDetectedPointA + lastDetectedPointB) / 2f;
        float distance = Vector2.Distance(lastDetectedPointA, lastDetectedPointB);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(center, $"Patrulla: {distance:F1}m");
#endif
    }
}
