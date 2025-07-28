using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    [Header("Resource Counts")]
    [SerializeField] private int Gold;
    [SerializeField] private int Silver;
    [SerializeField] private int Bronze;
    [SerializeField] private int ShinyPennies;

    [Header("Turnip Stats")]
    [SerializeField] private int Turnips = 0;
    [SerializeField] private int RoundTurnips = 0;

    [Header("Shortage Settings")]
    [SerializeField] private bool isInShortage = false;

    private const int shortageResourceLimit = 20;

    public event Action OnChanged;

    public int GoldValue => Gold;
    public int SilverValue => Silver;
    public int BronzeValue => Bronze;
    public int ShinyPenniesValue => ShinyPennies;
    public int TurnipsValue => Turnips;
    public int RoundTurnipsValue => RoundTurnips;
    public bool IsInShortage => isInShortage;

    public void Add(ResourceType type, int amount)
    {
        if (amount == 0) return;

        int newValue = 0;
        bool isMetal = IsMetal(type);

        if (isInShortage && isMetal && amount > 0)
        {
            int currentTotal = GetResourceAmount(type);
            if (currentTotal >= shortageResourceLimit) return;

            int allowed = shortageResourceLimit - currentTotal;
            amount = Mathf.Min(amount, allowed);
        }

        newValue = GetResourceAmount(type) + amount;
        if (newValue < 0) newValue = 0;

        switch (type)
        {
            case ResourceType.Gold: Gold = newValue; break;
            case ResourceType.Silver: Silver = newValue; break;
            case ResourceType.Bronze: Bronze = newValue; break;
            case ResourceType.ShinyPennies: ShinyPennies = newValue; break;
            case ResourceType.Turnips: Turnips = newValue; break;
            case ResourceType.RoundTurnips: RoundTurnips = newValue; break;
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
        amount >= 0 && GetResourceAmount(type) >= amount;

    private int GetResourceAmount(ResourceType type) =>
        type switch
        {
            ResourceType.Gold => Gold,
            ResourceType.Silver => Silver,
            ResourceType.Bronze => Bronze,
            ResourceType.ShinyPennies => ShinyPennies,
            ResourceType.Turnips => Turnips,
            ResourceType.RoundTurnips => RoundTurnips,
            _ => 0
        };

    private bool IsMetal(ResourceType type) =>
        type == ResourceType.Gold || type == ResourceType.Silver || type == ResourceType.Bronze;

    public void Notify() => OnChanged?.Invoke();

    public void Initialize(int gold = 40, int silver = 40, int bronze = 40, int shinyPennies = 50)
    {
        Gold = gold;
        Silver = silver;
        Bronze = bronze;
        ShinyPennies = shinyPennies;
        OnChanged?.Invoke();
    }

    public void SetShortage(bool state)
    {
        isInShortage = state;
        OnChanged?.Invoke(); // Notify UI of state change
    }
}