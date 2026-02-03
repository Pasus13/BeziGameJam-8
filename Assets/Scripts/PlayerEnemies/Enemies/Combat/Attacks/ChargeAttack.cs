using UnityEngine;

/// <summary>
/// Ataque de carga lineal para el Enemy Charge.
/// Fija la dirección hacia el jugador al iniciar y carga en línea recta sin corregir.
/// </summary>
public class ChargeAttack : MonoBehaviour, IAttack
{
    [Header("Configuration")]
    [SerializeField] private ChargeEnemyConfig config;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    [Header("Collision Detection")]
    [SerializeField] private LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private Vector2 chargeDirection;
    private float chargeTimer;
    private float chargeDistance;
    private Vector2 chargeStartPosition;
    private bool isCharging;

    public bool IsAttacking => isCharging;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError($"ChargeAttack on {gameObject.name}: Rigidbody2D not found!");
        }

        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Ground", "Wall", "Platform");
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
                if (context != null && context.Config is ChargeEnemyConfig chargeConfig)
                {
                    config = chargeConfig;
                }
            }
        }

        if (config == null)
        {
            Debug.LogError($"ChargeAttack on {gameObject.name}: ChargeEnemyConfig not found!");
        }
    }

    public bool CanStartAttack()
    {
        // Can charge if not already charging
        return !isCharging;
    }

    public void StartAttack()
    {
        if (isCharging || config == null) return;

        // Find player
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning($"ChargeAttack: Player not found!");
            return;
        }

        // SNAPSHOT: Lock direction towards where the player IS NOW
        chargeDirection = (player.position - transform.position).normalized;
        chargeStartPosition = transform.position;
        chargeTimer = 0f;
        chargeDistance = 0f;
        isCharging = true;

        if (showDebugLogs)
        {
            Debug.Log($"<color=orange>[{gameObject.name}]</color> 💨 Iniciando CARGA hacia {chargeDirection}");
        }
    }

    private void FixedUpdate()
    {
        if (!isCharging || config == null) return;

        // Aplicar velocidad CONSTANTE en la dirección fijada (NO corrige)
        rb.linearVelocity = chargeDirection * config.chargeSpeed;

        // Actualizar distancia recorrida y tiempo
        chargeDistance = Vector2.Distance(chargeStartPosition, transform.position);
        chargeTimer += Time.fixedDeltaTime;

        // Terminar si excedemos distancia máxima o duración máxima
        if (chargeDistance >= config.chargeMaxDistance ||
            chargeTimer >= config.chargeMaxDuration)
        {
            StopCharge();
        }
    }

    private void StopCharge()
    {
        if (!isCharging) return;

        isCharging = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[{gameObject.name}]</color> 🛑 Carga finalizada. Distancia: {chargeDistance:F2}m");
        }
    }

    public void CancelAttack()
    {
        StopCharge();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging) return;

        int collisionLayer = collision.gameObject.layer;

        // Check if layer is in obstacle mask
        if (((1 << collisionLayer) & obstacleLayer) != 0)
        {
            Debug.Log($"💥 Collision with {collision.gameObject.name}!");
            StopCharge();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!isCharging || config == null) return;

        // Dibujar dirección de carga
        Gizmos.color = Color.red;
        Vector3 endPoint = transform.position + (Vector3)chargeDirection * config.chargeMaxDistance;
        Gizmos.DrawLine(transform.position, endPoint);

        // Dibujar punto de inicio de carga
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(chargeStartPosition, 0.3f);
    }
}
