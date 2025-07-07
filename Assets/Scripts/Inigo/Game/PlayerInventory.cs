using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Resource Counts")]
    [SerializeField] public int Gold;
    [SerializeField] public int Silver;
    [SerializeField] public int Bronze;
    [SerializeField] public int ShinyPennies;

    [Header("Turnip Stats")]
    [SerializeField] public int Turnips = 0;
    [SerializeField] public int RoundTurnips = 0;

    [Header("Shortage Settings")]
    [SerializeField] public bool isInShortage = false;

    private int shortageResourceGainedThisRound = 0;
    private const int shortageResourceLimit = 20;

    public event Action OnChanged;

    public void Add(ResourceType type, int amount)
    {
        if (isInShortage && IsMetal(type))
        {
            int allowed = shortageResourceLimit - shortageResourceGainedThisRound;
            if (allowed <= 0) return;

            amount = Mathf.Min(amount, allowed);
            shortageResourceGainedThisRound += amount;
        }

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

    public void ResetShortageLimit() => shortageResourceGainedThisRound = 0;

    private bool IsMetal(ResourceType type) =>
        type == ResourceType.Gold || type == ResourceType.Silver || type == ResourceType.Bronze;

    public void Notify() => OnChanged?.Invoke();
}
