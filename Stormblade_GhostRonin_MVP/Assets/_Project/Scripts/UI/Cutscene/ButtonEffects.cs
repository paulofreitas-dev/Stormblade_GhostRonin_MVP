using UnityEngine;

public class BTNEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void OnActive()
    {
        animator.SetTrigger("active");
    }    
}

