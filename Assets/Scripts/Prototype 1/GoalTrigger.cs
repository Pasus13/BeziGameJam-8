using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool _alreadyTriggered;
    [SerializeField] private GameManager _gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_alreadyTriggered) return;

        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        _alreadyTriggered = true;

        _gameManager.OnLevelCompleted();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
            _alreadyTriggered = false;
    }
}
