using UnityEngine;

public class BreakableAnimationEvents : MonoBehaviour
{
    [SerializeField] private BreakableRewardSource breakableRewardSource;

    public void CompleteDestruction()
    {
        if(breakableRewardSource == null)
            return;

        breakableRewardSource.CompleteDestruction();
    }
}
