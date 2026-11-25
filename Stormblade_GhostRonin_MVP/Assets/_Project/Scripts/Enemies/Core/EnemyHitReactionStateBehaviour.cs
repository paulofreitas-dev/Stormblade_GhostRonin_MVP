using Unity.VisualScripting;
using UnityEngine;

public class EnemyHitReactionStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyHitReactionController hitReaction = animator.GetComponentInParent<EnemyHitReactionController>();

        if(hitReaction != null)
            hitReaction.BeginHitReaction();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyHitReactionController hitReaction = animator.GetComponentInParent<EnemyHitReactionController>();

        if(hitReaction != null)
            hitReaction.FinishHitReaction();
    }
}
