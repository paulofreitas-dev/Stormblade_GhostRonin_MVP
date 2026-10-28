using UnityEngine;

public class EnemyFacingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform attackHitbox;

    [Header("Sprite Orientation")]
    [Tooltip("Marque se a arte original olha para a direita.")]
    [SerializeField] private bool spriteFacesRightByDefault;

    [Header("Runtime Debug")]
    [SerializeField] private int currentDirection = -1;

    private float attackHitboxOriginalX;

    public int CurrentDirection => currentDirection;

    private void Awake()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if(attackHitbox != null)
            attackHitboxOriginalX = Mathf.Abs(attackHitbox.localPosition.x);
    }

    public void SetFacing(float direction)
    {
        if(Mathf.Approximately(direction, 0f))
            return;

        currentDirection = direction > 0f ? 1 : -1;

        UpdateVisual();
        UpdateAttackHitbox();
    }

    private void UpdateVisual()
    {
        if(spriteRenderer == null)
            return;

        bool lookingRight = currentDirection > 0;

        spriteRenderer.flipX = spriteFacesRightByDefault ? !lookingRight : lookingRight;
    }

    private void UpdateAttackHitbox()
    {
        if(attackHitbox == null)
            return;

        Vector3 localPosition = attackHitbox.localPosition;

        localPosition.x = attackHitboxOriginalX * currentDirection;

        attackHitbox.localPosition = localPosition;
    }
}
