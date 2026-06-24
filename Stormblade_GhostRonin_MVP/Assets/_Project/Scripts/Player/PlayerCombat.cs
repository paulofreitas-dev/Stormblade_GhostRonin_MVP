using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimationController playerAnimationController;

    [Header("Attack References")]
    [SerializeField] private Hitbox attackHitbox;

    [Header("Attack State")]
    [SerializeField] private bool isAttacking;

    private Vector3 attackHitboxBaseLocalPosition;

    public Hitbox AttackHitbox => attackHitbox;
    public bool IsAttacking => isAttacking;

    //belly e daniel estiveram aqui
    private void Awake()
    {
        if (attackHitbox == null)
        {
            Debug.LogWarning($"{gameObject.name}: attackHitbox não foi atribuído no PlayerCombat.");
        }

        if (playerInputReader == null)
        {
            Debug.LogWarning($"{gameObject.name}: playerInputReader não foi atribuído no PlayerCombat.");
        }

        if (playerMovement == null)
        {
            Debug.LogWarning($"{gameObject.name}: playerMovement não foi atribuído no PlayerCombat");
        }

        if (playerAnimationController == null)
        {
            Debug.LogWarning($"{gameObject.name}: playerAnimationController não foi atribuído no PlayerCombat");
        }

        if(attackHitbox != null)
        {
            attackHitbox.DisableHitbox();
        }

        if (attackHitbox != null)
        {
            attackHitboxBaseLocalPosition = attackHitbox.transform.localPosition;
        }

        if (playerMovement != null)
        {
            UpdateAttackHitboxDirection(playerMovement.IsFacingRight);
        }
    }

    private void Update()
    {
        HandleAttackRequest();

        if (playerMovement != null)
        {
            UpdateAttackHitboxDirection(playerMovement.IsFacingRight);
        }
    }

    private void HandleAttackRequest()
    {
        if (playerInputReader == null)
            return;

        if (!playerInputReader.AttackRequested)
            return;

        playerInputReader.ConsumeAttackRequest();

        if (!CanStartBasicAttack())
        {
            Debug.Log("PlayerCombat: pedido de ataque ignorado por regra de execução.");
            return;
        }

        StartBasicAttack();
    }

    private bool CanStartBasicAttack()
    {
        if (isAttacking)
            return false;

        if (playerMovement == null)
            return false;

        if (!playerMovement.IsGrounded)
            return false;

        return true;
    }

    private void StartBasicAttack()
    {
        isAttacking = true;

        if (playerAnimationController != null)
        {
            playerAnimationController.PlayAttack();
        }

        Debug.Log("PlayerCombat: ataque básico iniciado.");
    }

    public void EndBasicAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;
        Debug.Log("PlayerCombat: ataque básico encerrado.");
    }

    private void UpdateAttackHitboxDirection(bool isFacingRight)
    {
        if (attackHitbox == null)
            return;

        Vector3 localPosition = attackHitboxBaseLocalPosition;
        localPosition.x = Mathf.Abs(localPosition.x) * (isFacingRight ? 1f : -1f);

        attackHitbox.transform.localPosition = localPosition;
    }

    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.EnableHitbox();
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.DisableHitbox();
        }
    }

}
