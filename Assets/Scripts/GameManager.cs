using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { Playing, ChoosingModifier, GameOver }
    public State CurrentState { get; private set; } = State.Playing;

    [Header("References")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ModifierManager modifierManager;

    private ModifierData[] _currentOptions;
    private ModifierOffer[] _currentOffers;

    public HealthSystem HealthSystem => healthSystem;
    public PlayerMovement PlayerMovement => playerMovement;

    public void OnLevelCompleted()
    {
        CurrentState = State.ChoosingModifier;
        Time.timeScale = 0f;

        _currentOffers = modifierManager.GenerateOffers(3);

        UIManager.Instance.ShowModifierChoices(_currentOffers, OnModifierChosen);
    }

    private void OnModifierChosen(int index)
    {
        Time.timeScale = 1f;

        _currentOffers[index].Apply(this);

        ResetLevelKeepLives();
        CurrentState = State.Playing;
    }

    private void ResetLevelKeepLives()
    {
        // IMPORTANTE: NO resetear vidas aquí

        // Respawn player (resetea movimiento / posición)
        playerMovement.Respawn();

        // Resetear enemigos/obstáculos si tienes spawners
        // EnemySpawner.ResetAll();
        // ProjectileManager.Clear();
        // etc.
    }
}


