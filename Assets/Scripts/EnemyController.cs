using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AttackController))]
[RequireComponent(typeof(HealthController))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Setup")]
    [SerializeField] private Transform target;

    [Header("Components (assign in Inspector)")]
    [SerializeField] private MovementController movement;
    [SerializeField] private AttackController attackController;
    [SerializeField] private HealthController health;
    [SerializeField] private AnimationController animationController;

    private IMovable movementInterface;
    private IAttackable attackInterface;
    private IHealth healthInterface;
    private IAnimatable anim;

    private float currentSpeed;
    [SerializeField] private bool isDead;

    private bool prevInRange = false;

    private void Start()
    {
        movementInterface = movement as IMovable;
        attackInterface = attackController as IAttackable;
        healthInterface = health as IHealth;
        anim = animationController as IAnimatable;

        movementInterface.SetTarget(target);
        attackController.SetTarget(target);

        if (healthInterface != null)
        {
            healthInterface.OnDead += HandleDeath;
            healthInterface.OnHealthChanged += HandleHealthChanged;
        }

        if (attackInterface != null)
        {
            attackInterface.OnAttack += HandleAttackStart;
        }

        if (attackController != null)
        {
            attackController.OnAttackEnd += HandleAttackEnd;
        }

        currentSpeed = movement?.AnimationSpeed ?? 0f;
        prevInRange = attackController != null && attackController.IsTargetInRangeCached;
        ApplyMovementState(prevInRange);
    }

    private void OnDisable()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        if (healthInterface != null)
        {
            healthInterface.OnDead -= HandleDeath;
            healthInterface.OnHealthChanged -= HandleHealthChanged;
        }

        if (attackInterface != null)
        {
            attackInterface.OnAttack -= HandleAttackStart;
        }

        if (attackController != null)
        {
            attackController.OnAttackEnd -= HandleAttackEnd;
        }
    }

    private void Update()
    {
        if (isDead) return;

        movementInterface.MoveUpdate();

        bool inRange = attackController != null && attackController.IsTargetInRangeCached;

        if (inRange != prevInRange)
        {
            prevInRange = inRange;
            ApplyMovementState(inRange);
        }

        if (inRange)
        {
            attackInterface?.TryAttack();
        }

        currentSpeed = movement?.AnimationSpeed ?? currentSpeed;
    }

    private void ApplyMovementState(bool inRange)
    {
        if (inRange)
        {
            movementInterface.CanMove = false;
            anim?.PlayMove(false, 0f);
        }
        else
        {
            movementInterface.CanMove = true;
            anim?.PlayMove(true, currentSpeed);
        }
    }

    private void HandleAttackStart()
    {
        if (isDead) return;
        movementInterface.CanMove = false;
        anim?.PlayAttack();
    }

    private void HandleAttackEnd()
    {
        if (isDead) return;
        bool stillInRange = attackController != null && attackController.IsTargetInRangeCached;
        if (stillInRange)
        {
            movementInterface.CanMove = false;
            anim?.PlayMove(false, 0f);
            attackInterface?.TryAttack();
        }
        else
        {
            movementInterface.CanMove = true;
            anim?.StopAttack();
            anim?.PlayMove(true, currentSpeed);
        }
    }

    private void HandleHealthChanged(int cur, int max)
    {
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;
        anim?.PlayDie();
        movementInterface.CanMove = false;
        if (attackInterface != null) attackInterface.CanAttack = false;
        UnsubscribeAll();
    }

    public void OnDeathSink()
    {
        if (!isDead) return;

        float sinkDistance = 2f;
        float duration = 1.5f;

        transform.DOMoveY(transform.position.y - sinkDistance, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}
