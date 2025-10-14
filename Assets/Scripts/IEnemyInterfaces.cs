using System;
using UnityEngine;

public interface IMovable
{
    bool CanMove { get; set; }
    void SetTarget(Transform target);
    void MoveUpdate();
}

public interface IAttackable
{
    bool CanAttack { get; set; }
    float AttackRange { get; set; }
    void TryAttack();
    event Action OnAttack;
}

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    void TakeDamage(int amount);
    void Heal(int amount);
    event Action OnDead;
    event Action<int, int> OnHealthChanged;
}

public interface IAnimatable
{
    void PlayMove(bool moving, float speed = 0f);
    void PlayAttack();
    void StopAttack();
    void PlayDie();
    void PlayIdle();
}