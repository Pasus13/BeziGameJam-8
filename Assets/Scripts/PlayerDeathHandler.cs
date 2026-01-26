using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 0.05f;

    private HealthSystem _life;
    private PlayerMovement _movement;

    private void Awake()
    {
        _life = GetComponent<HealthSystem>();
        _movement = GetComponent<PlayerMovement>();

        _life.OnLifeLost.AddListener(HandleLifeLost);
        _life.OnGameOver.AddListener(HandleGameOver);
    }

    private void HandleLifeLost()
    {
        // Respawn inmediato (o con delay)
        Invoke(nameof(RespawnNow), respawnDelay);
    }

    private void RespawnNow()
    {
        _movement.Respawn();
    }

    private void HandleGameOver()
    {
        Debug.Log("GAME OVER");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
    }
}

