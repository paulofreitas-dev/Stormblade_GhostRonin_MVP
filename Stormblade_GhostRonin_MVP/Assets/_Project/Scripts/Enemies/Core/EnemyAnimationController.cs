using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int BasicAttackHash = Animator.StringToHash("attackBasic");

    private void Awake()
    {
        if(animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        if(animator == null)
            return;

        animator.SetBool(IsMovingHash, isMoving);
    }

    public void PlayBasicAttack()
    {
        if(animator == null)
            return;

        animator.SetTrigger(BasicAttackHash);
    }
}
