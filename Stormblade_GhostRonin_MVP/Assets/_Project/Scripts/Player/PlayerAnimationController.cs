using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    private static readonly int BaseStateHash = Animator.StringToHash("baseState");

    private PlayerBaseState currentBaseState;

    private void Update()
    {
        UpdateBaseState();
    }
    
    private void UpdateBaseState()
    {
        if (animator == null || playerMovement == null)
            return;

        PlayerBaseState targetBaseState = CalculateBaseState();

        if (targetBaseState != currentBaseState)
        {
            currentBaseState = targetBaseState;
            animator.SetInteger(BaseStateHash, (int)currentBaseState);
        }
    }

    private PlayerBaseState CalculateBaseState()
    {
        if (playerMovement.IsMovingHorizontally)
            return PlayerBaseState.Run;

        return PlayerBaseState.Idle;
    }   

}
