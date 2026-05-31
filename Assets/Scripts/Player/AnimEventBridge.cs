using UnityEngine;

public class AnimEventBridge : MonoBehaviour
{
    // PlayerAttack subscribes to this event in its Awake
    public event System.Action OnAttackHitEvent;
    public event System.Action OnFootstepEvent;

    // Called by Animation Event on the child sprite
    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }

    // Called by Animation Event on the walk animation
    public void OnFootstep()
    {
        OnFootstepEvent?.Invoke();
    }
}
