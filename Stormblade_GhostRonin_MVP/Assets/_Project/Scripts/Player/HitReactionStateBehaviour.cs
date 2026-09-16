using UnityEngine;

public class HitReactionStateBehaviour : StateMachineBehaviour
{
    private PlayerHitReaction hitReaction;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(hitReaction == null)
            hitReaction = animator.GetComponentInParent<PlayerHitReaction>();

        if(hitReaction != null)
            hitReaction.BeginHitReaction();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(hitReaction != null)
            hitReaction.FinishHitReaction();
    }
}
