using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack References")]
    [SerializeField] private Hitbox attackHitbox;
    [SerializeField] private PlayerInputReader inputReader;

    public Hitbox AttackHitbox => attackHitbox;

    //belly e daniel estiveram aqui
    private void Awake()
    {
        if (attackHitbox == null)
        {
            Debug.LogWarning($"{gameObject.name}: attackHitbox não foi atribuída no PlayerCombat.");
        }

        if (inputReader == null)
        {
            Debug.LogWarning($"{gameObject.name}: inputReader não foi atribuída no PlayerCombat.");
        }

        else
        {
            Debug.Log($"{gameObject}: PlayerCombat configurado com hitbox ofensiva.");
            attackHitbox.DisableHitbox();
        }
    }

    public void EnableAttackHitbox()
    {
        if(attackHitbox != null)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            attackHitbox.EnableHitbox();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            attackHitbox.DisableHitbox();
        }

    }
}
