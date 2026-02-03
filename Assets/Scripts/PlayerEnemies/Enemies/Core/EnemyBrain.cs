using UnityEngine;

/// <summary>
/// Cerebro del enemigo - Gestiona el FSM (Finite State Machine).
/// Controla las transiciones entre estados y coordina todos los componentes.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnemyConfigBase config;

    [Header("References")]
    [SerializeField] private Transform player;

    private EnemyContext context;
    private IEnemyState currentState;
    private Rigidbody2D rb;

    // Componentes modulares
    private IEnemyMovement movement;
    private EnemySensors sensors;
    private IAttack attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Encontrar player si no est� asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError($"EnemyBrain on {gameObject.name}: Player not found with tag 'Player'!");
            }
        }

        // Initialize components
        InitializeComponents();

        // Create shared context
        context = new EnemyContext(transform, player, rb, config);
        context.Movement = movement;
        context.Sensors = sensors;
        context.Attack = attack;

        // Initial state: Patrol
        ChangeState(new PatrolState(context, this));
    }

    private void InitializeComponents()
    {
        // Get or add components depending on what the enemy has
        movement = GetComponent<IEnemyMovement>();
        if (movement == null)
        {
            // If it doesn't have IEnemyMovement, add EnemyMovement by default
            movement = gameObject.AddComponent<EnemyMovement>();
            Debug.Log($"EnemyBrain: Added EnemyMovement automatically to {gameObject.name}");
        }

        sensors = GetComponent<EnemySensors>();
        if (sensors == null)
        {
            sensors = gameObject.AddComponent<EnemySensors>();
            Debug.Log($"EnemyBrain: Added EnemySensors automatically to {gameObject.name}");
        }

        attack = GetComponent<IAttack>();
        if (attack == null)
        {
            Debug.LogError($"EnemyBrain on {gameObject.name}: No IAttack component found! " +
                          "Add ChargeAttack, ShootAttack or DiveAttack.");
        }
    }

    private void Update()
    {
        if (currentState == null || context == null)
            return;

        // Tick del estado actual
        currentState.Tick(Time.deltaTime);

        // Tick del cooldown de ataque
        context.AttackCooldown.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Cambia el estado actual del FSM
    /// </summary>
    public void ChangeState(IEnemyState newState)
    {
        if (newState == null)
        {
            Debug.LogError($"EnemyBrain: Intentando cambiar a un estado NULL!");
            return;
        }

        // Salir del estado actual
        currentState?.Exit();

        // Cambiar al nuevo estado
        currentState = newState;

        // Entrar al nuevo estado
        currentState.Enter();

        Debug.Log($"<color=cyan>[{gameObject.name}]</color> Estado: <b>{newState.GetType().Name}</b>");
    }

    /// <summary>
    /// Obtiene el contexto (�til para debugging)
    /// </summary>
    public EnemyContext GetContext()
    {
        return context;
    }

    private void OnDrawGizmosSelected()
    {
        if (config == null) return;

        // Dibujar rango de detecci�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, config.detectionRange);

        // Dibujar spawn position
        if (Application.isPlaying && context != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(context.SpawnPosition, 0.3f);
            Gizmos.DrawLine(transform.position, context.SpawnPosition);
        }
    }
}
