using UnityEngine;

/// <summary>
/// Manager singleton que proporciona acceso a los límites de la arena.
/// Usa el BoxCollider2D del GameObject de arena para definir límites.
/// </summary>
public class ArenaLimits : MonoBehaviour
{
    public static ArenaLimits Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private BoxCollider2D arenaBounds;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Verificar que tenemos el collider
        if (arenaBounds == null)
        {
            Debug.LogError("ArenaLimits: No hay BoxCollider2D asignado!");
        }
    }

    /// <summary>
    /// Obtiene los límites de la arena
    /// </summary>
    public Bounds GetBounds()
    {
        if (arenaBounds == null)
        {
            Debug.LogError("ArenaLimits: arenaBounds es null!");
            return new Bounds(Vector3.zero, Vector3.one * 100f);
        }

        return arenaBounds.bounds;
    }

    /// <summary>
    /// Verifica si una posición está dentro de la arena
    /// </summary>
    public bool IsInsideArena(Vector2 position)
    {
        if (arenaBounds == null) return true;

        return arenaBounds.bounds.Contains(position);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || arenaBounds == null) return;

        // Dibujar límites de arena
        Gizmos.color = Color.yellow;
        Bounds bounds = arenaBounds.bounds;

        // Dibujar rectángulo de límites
        Vector3 topLeft = new Vector3(bounds.min.x, bounds.max.y, 0);
        Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y, 0);
        Vector3 bottomLeft = new Vector3(bounds.min.x, bounds.min.y, 0);
        Vector3 bottomRight = new Vector3(bounds.max.x, bounds.min.y, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
