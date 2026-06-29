using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;

    [Header("Air State Timing")]
    [SerializeField] private float jumpStartHoldTime = 0.10f;
    [SerializeField] private float jumpLandingHoldTime = 0.8f;
    
    private static readonly int BaseStateHash = Animator.StringToHash("baseState");
    private static readonly int AttackHash = Animator.StringToHash("attackBasic");

    private PlayerBaseState currentBaseState = PlayerBaseState.Idle;

    private bool isTransientStateActive;
    private float transientStateTimer;
    private PlayerBaseState transientState;

    private void Update()
    {
        UpdateBaseState();
    }

    private bool ShouldInterruptLandingWithJump()
    {
        return isTransientStateActive &&
               transientState == PlayerBaseState.JumpLanding &&
               playerMovement != null &&
               playerMovement.JumpStartedThisFrame;
    }
    
    private void UpdateBaseState()
    {
        if (animator == null || playerMovement == null)
            return;

        if (playerMovement.LandedThisFrame)
            SetTransientState(PlayerBaseState.JumpLanding, jumpStartHoldTime);

        else if (playerMovement.JumpStartedThisFrame)
            SetTransientState(PlayerBaseState.JumpStart, jumpStartHoldTime);

        // prioridade: se durante o landing o jogador já pediu novo pulo, 
        // o landing é cancelado imediatamente
        if (ShouldInterruptLandingWithJump())
        {
            SetTransientState(PlayerBaseState.JumpStart, jumpStartHoldTime);
        }

        UpdateTransientTimer();

        PlayerBaseState targetBaseState = CalculateBaseState();

        if (targetBaseState != currentBaseState)
        {
            currentBaseState = targetBaseState;
            animator.SetInteger(BaseStateHash, (int)currentBaseState);
        }
    }

    private void UpdateTransientTimer()
    {
        if (!isTransientStateActive)
            return;

        transientStateTimer -= Time.deltaTime;

        if (transientStateTimer <= 0f)
            isTransientStateActive = false;
    }

    private void SetTransientState(PlayerBaseState state, float duration)
    {
        transientState = state;
        transientStateTimer = duration;
        isTransientStateActive = true;
    }

    private PlayerBaseState CalculateBaseState()
    {
        if (isTransientStateActive)
            return transientState;

        if (playerMovement.IsAirborne)
            return PlayerBaseState.JumpAir;

        if (playerMovement.IsCrouching)
            return PlayerBaseState.Crouch;

        if (playerMovement.IsMovingHorizontally)
            return PlayerBaseState.Run;

        return PlayerBaseState.Idle;
    }

    public void PlayAttack()
    {
        if (animator == null)
            return;

        animator.SetTrigger(AttackHash);
    }

    public void EndBasicAttack()
    {
        if (playerCombat != null)
        {
            playerCombat.EndBasicAttack();
        }
    }

    public void EnableAttackHitbox()
    {
        if (playerCombat != null)
        {
            playerCombat.EnableAttackHitbox();
        }
    }

    public void DisableAttackHitbox()
    {
        if (playerCombat != null)
        {
            playerCombat.DisableAttackHitbox();
        }
    }

}
