using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputReader : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Debug")]
    [SerializeField] private float moveInputX;

    public float MoveInputX => moveInputX;

    private void OnEnable()
    {
        if(moveAction != null)
        {
            moveAction.action.Enable();
        }

    }

    private void OnDisable()
    {
        if(moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null)
        {
            moveInputX = 0f;
            return;
        }

        Vector2 moveValue = moveAction.action.ReadValue<Vector2>();

        moveInputX = moveValue.x;
    }

}
