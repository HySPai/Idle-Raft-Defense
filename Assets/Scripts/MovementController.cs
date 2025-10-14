using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MovementController : MonoBehaviour, IMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    private Transform target;
    public bool CanMove { get; set; } = true;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void MoveUpdate()
    {
        if (target == null || !CanMove) return;
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }
        Vector3 dirNorm = direction.normalized;
        transform.position += dirNorm * moveSpeed * Time.deltaTime;
        Quaternion lookRotation = Quaternion.LookRotation(dirNorm);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
