using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;       // Player
    [SerializeField] private Transform cameraPivot;  // Pivot

    [Header("Settings")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float smooth = 10f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float sphereRadius = 0.25f;

    void LateUpdate()
    {
        Vector3 origin = cameraPivot.position;

        Vector3 direction = (transform.position - origin).normalized;

        float targetDistance = maxDistance;

        if (Physics.SphereCast(
            origin,
            sphereRadius,
            direction,
            out RaycastHit hit,
            maxDistance,
            collisionMask))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        Vector3 finalPos = origin + direction * targetDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            finalPos,
            Time.deltaTime * smooth
        );
    }
}