using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Puede ser que el collider que entra sea feet/body,
        // así que buscamos en el parent.
        var movement = other.GetComponentInParent<PlayerMovement>();
        if (movement != null)
        {
            movement.Respawn();
        }
    }
}

