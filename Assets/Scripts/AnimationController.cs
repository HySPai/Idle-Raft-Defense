using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour, IAnimatable
{
    [SerializeField] Animator animator;

    private readonly int hashIsMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int hashIsDead = Animator.StringToHash("IsDead");

    private bool currentIsMove;
    private bool currentIsAttack;
    private float currentSpeed;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        currentIsMove = animator.GetBool(hashIsMove);
        currentIsAttack = animator.GetBool(hashIsAttack);
        currentSpeed = animator.GetFloat(hashSpeed);
    }

    public void PlayMove(bool moving, float speed = 0f)
    {
        if (moving)
        {
            if (!currentIsMove)
            {
                animator.SetBool(hashIsMove, true);
                currentIsMove = true;
            }
            if (Mathf.Abs(currentSpeed - speed) > 0.01f)
            {
                animator.SetFloat(hashSpeed, speed);
                currentSpeed = speed;
            }
        }
        else
        {
            if (currentIsMove)
            {
                animator.SetBool(hashIsMove, false);
                currentIsMove = false;
            }
            if (currentSpeed != 0f)
            {
                animator.SetFloat(hashSpeed, 0f);
                currentSpeed = 0f;
            }
        }
    }

    public void PlayAttack()
    {
        if (!currentIsAttack)
        {
            animator.SetBool(hashIsAttack, true);
            currentIsAttack = true;
        }
    }

    public void StopAttack()
    {
        if (currentIsAttack)
        {
            animator.SetBool(hashIsAttack, false);
            currentIsAttack = false;
        }
    }

    public void PlayDie()
    {
        animator.SetBool(hashIsDead, true);
    }

    public void PlayIdle()
    {
        if (animator == null) return;
        if (currentIsMove)
        {
            animator.SetBool(hashIsMove, false);
            currentIsMove = false;
        }
        animator.SetFloat(hashSpeed, 0f);
        if (currentIsAttack)
        {
            animator.SetBool(hashIsAttack, false);
            currentIsAttack = false;
        }
    }
}
