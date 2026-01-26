using UnityEngine;

public class KillZone : MonoBehaviour
{
    private bool _alreadyTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_alreadyTriggered) return;

        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        _alreadyTriggered = true;

        var life = root.GetComponent<HealthSystem>();
        if (life != null)
            life.LoseLife();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
            _alreadyTriggered = false;
    }

}

