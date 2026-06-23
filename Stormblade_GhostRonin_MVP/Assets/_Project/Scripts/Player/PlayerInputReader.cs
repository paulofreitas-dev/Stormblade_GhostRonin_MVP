using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputReader : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference attackAction;

    [Header("Debug")]
    [SerializeField] private float moveInputX;
    [SerializeField] private bool jumpRequested;
    [SerializeField] private bool attackRequested;

    public float MoveInputX => moveInputX;
    public bool JumpRequested => jumpRequested;
    public bool AttackRequested => attackRequested;

    private void OnEnable()
    {
        if(moveAction != null)
        {
            moveAction.action.Enable();
        }

        if(jumpAction != null)
        {
            jumpAction.action.Enable();
        }

        if (attackAction != null)
        {
            attackAction.action.Enable();
        }

    }

    private void OnDisable()
    {
        if(moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.Disable();
        }

        if (attackAction != null)
        {
            attackAction.action.Disable();
        }
    }

    private void Update()
    {
        ReadMoveInput();

        ReadJumpInput();

        ReadAttackInput();
    }

    private void ReadMoveInput()
    {
        if (moveAction == null)
        {
            moveInputX = 0f;
            return;
        }

        Vector2 moveValue = moveAction.action.ReadValue<Vector2>();
        moveInputX = moveValue.x;
    }

    private void ReadJumpInput()
    {
        if (jumpAction == null)
        {
            jumpRequested = false;
            return;
        }

        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

    }

    private void ReadAttackInput()
    {
        if (attackAction == null)
        {
            attackRequested = false;
            return;
        }

        if (attackAction.action.WasPressedThisFrame())
        {
            attackRequested = true;
            Debug.Log("Pedido de ataque registrado.");
        }
    }

    public void ConsumeJumpRequest()
    {
        jumpRequested = false;
    }

    public void ConsumeAttackRequest()
    {
        attackRequested = false;
    }

}
