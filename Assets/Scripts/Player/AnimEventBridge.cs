using UnityEngine;

public class AnimEventBridge : MonoBehaviour
{
    // PlayerAttack subscribes to this event in its Awake
    public event System.Action OnAttackHitEvent;

    // Called by Animation Event on the child sprite
    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }
}
