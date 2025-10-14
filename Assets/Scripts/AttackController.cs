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

    private float cooldownTimer = 0f;
    private Transform target;

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

    public event Action OnAttack;
    public event Action<int> OnDealDamage;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public void TryAttack()
    {
        if (!canAttack || target == null) return;
        if (!IsTargetInRange()) return;
        if (cooldownTimer <= 0f)
        {
            DoAttack();
            cooldownTimer = attackCooldown;
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
        OnDealDamage?.Invoke(damage);
    }
}
