using System;
using UnityEngine;

public class PlayerWater : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float drainRate = 2f;         // units per second
    [SerializeField] private float dehydrationDamage = 5f; // health lost per second when dry

    // Events for UI and other systems
    public event Action<float, float> OnWaterChanged; // current, max
    public event Action OnDehydrated;
    public event Action OnWaterRestored;

    public float MaxWater => maxWater;
    public float CurrentWater { get; private set; }
    public bool IsDehydrated { get; private set; }

    private PlayerHealth _health;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        CurrentWater = maxWater;
        OnWaterChanged?.Invoke(CurrentWater, MaxWater);
    }

    private void Update()
    {
        DrainWater();
        ApplyDehydrationDamage();
    }

    private void DrainWater()
    {
        if (CurrentWater <= 0) return;

        CurrentWater = Mathf.Clamp(CurrentWater - drainRate * Time.deltaTime, 0f, maxWater);
        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        // Transition to dehydration
        if (CurrentWater <= 0 && !IsDehydrated)
        {
            IsDehydrated = true;
            OnDehydrated?.Invoke();
        }
    }

    private void ApplyDehydrationDamage()
    {
        if (!IsDehydrated || _health == null) return;
        _health.TakeDamage(dehydrationDamage * Time.deltaTime);
    }

    public void Replenish(float amount)
    {
        if (amount <= 0) return;

        bool wasDehydrated = IsDehydrated;

        CurrentWater = Mathf.Clamp(CurrentWater + amount, 0f, maxWater);
        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        // Recover from dehydration
        if (wasDehydrated && CurrentWater > 0)
        {
            IsDehydrated = false;
            OnWaterRestored?.Invoke();
        }
    }
}
