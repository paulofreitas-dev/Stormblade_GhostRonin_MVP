using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputReader : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Debug")]
    [SerializeField] private float moveInputX;
    [SerializeField] private bool jumpRequested;

    public float MoveInputX => moveInputX;

    public bool JumpRequested => jumpRequested;

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
    }

    private void Update()
    {
        ReadMoveInput();

        ReadJumpInput();
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

    public void ConsumeJumpRequest()
    {
        jumpRequested = false;
    }

}
