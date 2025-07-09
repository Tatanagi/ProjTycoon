using UnityEngine;

/// <summary>
/// Each round, one resource type is worth extra value when exchanged.
/// </summary>
public class RoyalDecree : MonoBehaviour
{
    public static RoyalDecree Instance { get; private set; }

    public ResourceType favoredType { get; private set; }

    public int multiplier { get; private set; } = 3;

    // ---------- Public API ----------

    public void GenerateNewDecree()
    {
        favoredType = (ResourceType)Random.Range(0, 3);
        multiplier = Random.Range(2, 5);

        Debug.Log($"Royal Decree: {favoredType} is worth x{multiplier}!");
    }

    public int GetValueExchange(ResourceType type, int amount)
    {
        int baseValue = type switch
        {
            ResourceType.Bronze => 1,
            ResourceType.Silver => 3,
            ResourceType.Gold => 6,
            _ => 0
        };

        if (type == favoredType) baseValue *= multiplier;
        return baseValue * amount;
    }
}
