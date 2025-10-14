using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour, IAnimatable
{
    private Animator animator;

    private readonly int hashIsMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int hashIsDead = Animator.StringToHash("IsDead");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayMove(bool moving, float speed = 0f)
    {
        if (animator == null) return;
        animator.SetBool(hashIsMove, moving);
        animator.SetFloat(hashSpeed, speed);
    }

    public void PlayAttack()
    {
        if (animator == null) return;
        animator.SetBool(hashIsAttack, true);
    }

    public void StopAttack()
    {
        if (animator == null) return;
        animator.SetBool(hashIsAttack, false);
    }

    public void PlayDie()
    {
        if (animator == null) return;
        animator.SetBool(hashIsDead, true);
    }

    public void PlayIdle()
    {
        if (animator == null) return;
        animator.SetBool(hashIsMove, false);
        animator.SetFloat(hashSpeed, 0f);
        animator.SetBool(hashIsAttack, false);
    }
}
