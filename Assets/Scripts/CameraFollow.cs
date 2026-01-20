using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float deadZoneWidth = 1f;
    public float deadZoneHeight = 1f;
    public float smoothSpeed = 5f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        float deltaX = targetPosition.x - currentPosition.x;
        float deltaY = targetPosition.y - currentPosition.y;

        float moveX = 0f;
        float moveY = 0f;

        if (Mathf.Abs(deltaX) > deadZoneWidth / 2f)
        {
            moveX = deltaX - Mathf.Sign(deltaX) * deadZoneWidth / 2f;
        }

        if (Mathf.Abs(deltaY) > deadZoneHeight / 2f)
        {
            moveY = deltaY - Mathf.Sign(deltaY) * deadZoneHeight / 2f;
        }

        Vector3 desiredPosition = currentPosition + new Vector3(moveX, moveY, 0f);
        desiredPosition.z = -10f;

        transform.position = Vector3.Lerp(currentPosition, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
