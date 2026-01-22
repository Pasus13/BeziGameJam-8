using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset (player lower = more view above)")]
    [SerializeField] private Vector2 offset = new Vector2(0f, -2f);

    [Header("Dead Zone (in world units)")]
    [Tooltip("Width/Height of the dead zone rectangle in WORLD units.")]
    [SerializeField] private Vector2 deadZoneSize = new Vector2(4f, 2f);

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 _velocity;

    private void LateUpdate()
    {
        if (target == null) return;

        // 1) Target position with offset (this is what we want to keep inside the dead zone)
        Vector3 focus = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // 2) Current camera center (world)
        Vector3 camPos = transform.position;

        // 3) Build dead zone bounds around current camera position
        float halfW = deadZoneSize.x * 0.5f;
        float halfH = deadZoneSize.y * 0.5f;

        float minX = camPos.x - halfW;
        float maxX = camPos.x + halfW;
        float minY = camPos.y - halfH;
        float maxY = camPos.y + halfH;

        // 4) If focus is outside dead zone, move camera just enough to bring it back to the edge
        float newX = camPos.x;
        float newY = camPos.y;

        if (focus.x < minX) newX += focus.x - minX;
        else if (focus.x > maxX) newX += focus.x - maxX;

        if (focus.y < minY) newY += focus.y - minY;
        else if (focus.y > maxY) newY += focus.y - maxY;

        Vector3 desired = new Vector3(newX, newY, camPos.z);

        // 5) Smooth
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw dead zone rectangle in Scene view (approx, at current camera position)
        Gizmos.color = Color.yellow;

        Vector3 camPos = Application.isPlaying ? transform.position : transform.position;

        Vector3 size = new Vector3(deadZoneSize.x, deadZoneSize.y, 0f);
        Gizmos.DrawWireCube(camPos, size);
    }
#endif
}

