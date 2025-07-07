using System;
using UnityEngine;

/// <summary>
/// Keeps all resource counts for one player and notifies listeners when they change.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public int Gold { get; private set; }
    public int Silver { get; private set; }
    public int Bronze { get; private set; }
    public int ShinyPennies { get; private set; }
    public int Turnips { get; set; } = 0;
    public int RoundTurnips { get; set; } = 0;

    public event Action OnChanged;

    /* ------------ Public API ------------ */

    public void Add(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold: Gold += amount; break;
            case ResourceType.Silver: Silver += amount; break;
            case ResourceType.Bronze: Bronze += amount; break;
            case ResourceType.ShinyPennies: ShinyPennies += amount; break;
        }
        OnChanged?.Invoke();
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (!CanAfford(type, amount)) return false;
        Add(type, -amount);
        return true;
    }

    public bool CanAfford(ResourceType type, int amount) =>
        type switch
        {
            ResourceType.Gold => Gold >= amount,
            ResourceType.Silver => Silver >= amount,
            ResourceType.Bronze => Bronze >= amount,
            ResourceType.ShinyPennies => ShinyPennies >= amount,
            _ => false
        };

    public void Notify() => OnChanged?.Invoke();
}
