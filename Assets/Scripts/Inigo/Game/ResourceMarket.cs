using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ResourceType { Bronze, Silver, Gold, ShinyPennies, Turnips, RoundTurnips }

[System.Serializable]
public class ResourceToken
{
    public ResourceType type;
    public int quantity;
}

/// <summary>Generates and dispenses resource tokens each round.</summary>
public class ResourceMarket : MonoBehaviour
{
    public List<ResourceToken> availableTokens = new();

    [Header("Start Tile Bonus")]
    public int startGold = 5;
    public int startSilver = 5;
    public int startBronze = 5;

    public void GenerateTokens(int round)
    {
        availableTokens.Clear();

        availableTokens.Add(new ResourceToken { type = ResourceType.Bronze, quantity = 5 + round });
        availableTokens.Add(new ResourceToken { type = ResourceType.Silver, quantity = 3 + round / 2 });
        availableTokens.Add(new ResourceToken { type = ResourceType.Gold, quantity = 1 + round / 3 });

        Debug.Log($"New tokens added to the market: {availableTokens.Count} types generated for round {round}.");
    }

    public void GiveResourceToPlayer(PlayerController player, int roll)
    {
        if (player == null || player.inventory == null) return;

        if (availableTokens.Count == 0)
        {
            Debug.LogWarning("No tokens available in ResourceMarket!");
            return;
        }

        int totalWeight = availableTokens.Sum(token => token.quantity);
        int rollThreshold = (int)(roll / 6f * totalWeight);

        int cumulative = 0;
        foreach (var token in availableTokens)
        {
            cumulative += token.quantity;
            if (rollThreshold <= cumulative)
            {
                player.inventory.Add(token.type, 1);
                Debug.Log($"{player.name} gained 1 {token.type} based on roll {roll}.");
                return;
            }
        }

        player.inventory.Add(ResourceType.Bronze, 1);
        Debug.Log($"{player.name} gained 1 Bronze (fallback) for roll {roll}.");
    }

    public void GiveStartTileBonus(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.Gold, startGold);
        player.inventory.Add(ResourceType.Silver, startSilver);
        player.inventory.Add(ResourceType.Bronze, startBronze);

        Debug.Log($"{player.name} gained {startGold} Gold, {startSilver} Silver, {startBronze} Bronze for passing Start.");
    }
}