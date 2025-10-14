using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AttackController))]
[RequireComponent(typeof(HealthController))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Setup")]
    [SerializeField] private Transform target;

    private IMovable movement;
    private IAttackable attack;
    private IHealth health;
    private IAnimatable anim;
    private AttackController attackController;

    private Vector3 lastPosition;
    private float currentSpeed;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        attack = GetComponent<AttackController>();
        attackController = attack as AttackController;
        health = GetComponent<HealthController>();
        anim = GetComponentInChildren<IAnimatable>() as IAnimatable;

        if (movement != null) movement.SetTarget(target);
        if (attackController != null) attackController.SetTarget(target);

        if (health != null)
        {
            health.OnDead += HandleDeath;
            health.OnHealthChanged += HandleHealthChanged;
        }

        if (attack != null)
        {
            attack.OnAttack += HandleAttack;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (target == null) return;

        bool inRange = attackController != null && attackController.IsTargetInRange();

        if (inRange)
        {
            if (movement != null) movement.CanMove = false;
            if (anim != null)
            {
                anim.PlayMove(false, 0f);
                anim.PlayAttack();
            }
            if (attack != null) attack.TryAttack();
            RotateTowardsTarget();
        }
        else
        {
            if (movement != null)
            {
                movement.CanMove = true;
                movement.MoveUpdate();
            }

            UpdateSpeed();
            if (anim != null)
            {
                anim.StopAttack();
                anim.PlayMove(true, currentSpeed);
            }
        }
    }

    private void UpdateSpeed()
    {
        Vector3 delta = transform.position - lastPosition;
        currentSpeed = delta.magnitude / Time.deltaTime;
        lastPosition = transform.position;
    }

    private void RotateTowardsTarget()
    {
        if (target == null) return;
        Vector3 dir = (target.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
    }

    private void HandleAttack()
    {
        if (anim != null) anim.PlayAttack();
    }

    private void HandleHealthChanged(int cur, int max)
    {
    }

    private void HandleDeath()
    {
        if (anim != null) anim.PlayDie();
        if (movement != null) movement.CanMove = false;
        if (attack != null) attack.CanAttack = false;
        Destroy(gameObject, 2f);
    }
}
