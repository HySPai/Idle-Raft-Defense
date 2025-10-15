using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MovementController : MonoBehaviour, IMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private float animationSpeed = 1f;

    private Transform target;
    public bool CanMove { get; set; } = true;

    [SerializeField] private AnimationController anim;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public float AnimationSpeed => Mathf.Max(0f, animationSpeed);

    public void MoveUpdate()
    {
        if (target == null) return;

        if (!CanMove)
        {
            return;
        }

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            anim?.PlayMove(false, 0f);
            return;
        }

        Vector3 dirNorm = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dirNorm);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        anim?.PlayMove(true, animationSpeed);
    }
}
