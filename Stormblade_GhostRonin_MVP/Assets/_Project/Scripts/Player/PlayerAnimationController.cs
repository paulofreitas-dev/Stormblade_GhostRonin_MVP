using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Air State Timing")]
    [SerializeField] private float jumpStartHoldTime = 0.10f;
    [SerializeField] private float jumpLandingHoldTime = 0.8f;
    
    private static readonly int BaseStateHash = Animator.StringToHash("baseState");
    private static readonly int AttackHash = Animator.StringToHash("attackBasic");

    private PlayerBaseState currentBaseState = PlayerBaseState.Idle;

    private bool isTransientStateActive;
    private float transientStateTimer;
    private PlayerBaseState transientState;

    private bool wasGroundedLastUpdate;

    private void Awake()
    {
        if (playerMovement != null)
            wasGroundedLastUpdate = playerMovement.IsGrounded;
    }

    private void Update()
    {
        UpdateBaseState();
    }

    private bool ShouldInterruptLandingWithJump()
    {
        return isTransientStateActive &&
               transientState == PlayerBaseState.JumpLanding &&
               playerMovement != null &&
               playerMovement.IsGrounded &&
               playerMovement.HasJumpRequest;
    }
    
    private void UpdateBaseState()
    {
        if (animator == null || playerMovement == null)
            return;

        bool isGroundedNow = playerMovement.IsGrounded;
        bool justLeftGrounded = wasGroundedLastUpdate && !isGroundedNow;
        bool justLanded = !wasGroundedLastUpdate && isGroundedNow;

        if (justLanded)
            SetTransientState(PlayerBaseState.JumpLanding, jumpLandingHoldTime);

        else if (justLeftGrounded && playerMovement.VerticalVelocity > 0.01f)
            SetTransientState(PlayerBaseState.JumpStart, jumpStartHoldTime);

        // prioridade: se durante o landing o jogador já pediu novo pulo, 
        // o landing é cancelado imediatamente
        if (ShouldInterruptLandingWithJump())
        {
            SetTransientState(PlayerBaseState.JumpStart, jumpStartHoldTime);
        }

        wasGroundedLastUpdate = isGroundedNow;

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

}
