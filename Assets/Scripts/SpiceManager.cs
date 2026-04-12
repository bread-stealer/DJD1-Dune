using System;
using UnityEngine;

// Singleton that tracks total spice collected across the level
public class SpiceManager : MonoBehaviour
{
    public static SpiceManager Instance {get; private set;}

    public event Action<int> OnSpiceChanged;
    public int TotalSpice {get; private set;}

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddSpice(int amount)
    {
        if (amount <= 0) return;
        TotalSpice += amount;
        OnSpiceChanged?.Invoke(TotalSpice);
    }
}
