using System;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class AttackController : MonoBehaviour, IAttackable
{
    [Header("Attack Settings")]
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float rangeHysteresis = 0.15f;
    [SerializeField] private float hitTolerance = 0.05f;

    private float cooldownTimer = 0f;
    private Transform target;
    private bool isAttacking = false;
    private bool isTargetInRangeCached = false;

    public bool CanAttack
    {
        get => canAttack;
        set => canAttack = value;
    }

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }

    public bool IsTargetInRangeCached => isTargetInRangeCached;

    public event Action OnAttack;
    public event Action<int> OnDealDamage;
    public event Action OnAttackEnd;

    public void SetTarget(Transform t)
    {
        target = t;
        RefreshRangeNow();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        UpdateCachedRange();
    }

    private void UpdateCachedRange()
    {
        if (target == null)
        {
            isTargetInRangeCached = false;
            return;
        }

        float sqrDist = (transform.position - target.position).sqrMagnitude;
        float range = attackRange;
        float lower = (range - rangeHysteresis) * (range - rangeHysteresis);
        float upper = (range + rangeHysteresis) * (range + rangeHysteresis);

        if (isTargetInRangeCached)
        {
            if (sqrDist > upper) isTargetInRangeCached = false;
        }
        else
        {
            if (sqrDist <= lower) isTargetInRangeCached = true;
        }
    }

    public void RefreshRangeNow()
    {
        if (target == null)
        {
            isTargetInRangeCached = false;
            return;
        }
        float sqrDist = (transform.position - target.position).sqrMagnitude;
        float range = attackRange;
        isTargetInRangeCached = sqrDist <= (range * range);
    }

    public void TryAttack()
    {
        if (!canAttack) return;
        if (isAttacking) return;

        RefreshRangeNow();

        if (!IsTargetInRangeCached) return;
        if (cooldownTimer <= 0f)
        {
            DoAttack();
            cooldownTimer = attackCooldown;
            isAttacking = true;
        }
    }

    public bool IsTargetInRange()
    {
        if (target == null) return false;
        float dist = Vector3.Distance(transform.position, target.position);
        return dist <= attackRange;
    }

    private void DoAttack()
    {
        OnAttack?.Invoke();
    }

    public void AttackHitEvent()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange + hitTolerance)
        {
            OnDealDamage?.Invoke(damage);
        }
    }

    public void AttackEndEvent()
    {
        isAttacking = false;
        RefreshRangeNow();
        OnAttackEnd?.Invoke();
        Debug.Log($"{gameObject.name} attack animation ended, reset attacking state");
    }

    public int GetDamage()
    {
        return damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
