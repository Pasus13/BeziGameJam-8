using UnityEngine;

/// <summary>
/// Ataque de disparo en arco (mortero) para el Enemy Shooter.
/// Apunta a donde ESTÁ el jugador y dispara un proyectil en arco.
/// </summary>
public class ShootAttack : MonoBehaviour, IAttack
{
    [Header("Configuration")]
    [SerializeField] private ShooterEnemyConfig config;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private bool isShooting;
    private Vector2 targetPosition;

    public bool IsAttacking => isShooting;

    private void Start()
    {
        // Obtener config si no está asignado
        if (config == null)
        {
            EnemyBrain brain = GetComponent<EnemyBrain>();
            if (brain != null)
            {
                var context = brain.GetContext();
                if (context != null && context.Config is ShooterEnemyConfig shooterConfig)
                {
                    config = shooterConfig;
                }
            }
        }

        if (config == null)
        {
            Debug.LogError($"ShootAttack on {gameObject.name}: ShooterEnemyConfig not found!");
        }
    }

    public bool CanStartAttack()
    {
        return !isShooting &&
               config != null &&
               config.projectilePrefab != null;
    }

    public void StartAttack()
    {
        if (isShooting || config == null) return;

        // Find player
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning($"ShootAttack: Player not found!");
            return;
        }

        // SNAPSHOT: Lock position towards where the player IS NOW
        targetPosition = player.position;
        isShooting = true;

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[{gameObject.name}]</color> 🎯 Apuntando a {targetPosition}");
        }
    }

    /// <summary>
    /// Ejecuta el disparo (llamado por AttackState después de la telegrafía)
    /// </summary>
    public void ExecuteShot()
    {
        if (!isShooting || config == null) return;

        // Verificar que tenemos el prefab del proyectil
        if (config.projectilePrefab == null)
        {
            Debug.LogError($"ShootAttack: No hay proyectil asignado en el config!");
            isShooting = false;
            return;
        }

        // Instanciar proyectil
        GameObject projectileObj = Instantiate(
            config.projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        // Configurar proyectil
        MortarProjectile projectile = projectileObj.GetComponent<MortarProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                targetPosition,
                config.projectileSpeed,
                config.arcHeight,
                config.projectileDamage,
                config.projectileLifetime
            );
        }
        else
        {
            Debug.LogError($"ShootAttack: El proyectil no tiene componente MortarProjectile!");
            Destroy(projectileObj);
        }

        isShooting = false;

        if (showDebugLogs)
        {
            Debug.Log($"<color=green>[{gameObject.name}]</color> 🚀 Proyectil disparado");
        }
    }

    public void CancelAttack()
    {
        isShooting = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isShooting) return;

        // Dibujar objetivo
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
