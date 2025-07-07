using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Bronze, Silver, Gold, ShinyPennies }

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

        Debug.Log("New tokens added to the market.");
    }

    public void GiveResourceToPlayer(PlayerController player, int roll)
    {
        switch (roll)
        {
            case <= 2:
                player.inventory.Add(ResourceType.Bronze, 2);
                Debug.Log($"{player.name} gained Bronze x2.");
                break;
            case <= 5: // Change to 5
                player.inventory.Add(ResourceType.Silver, 1);
                Debug.Log($"{player.name} gained Silver x1.");
                break;
            default:
                player.inventory.Add(ResourceType.Gold, 1);
                Debug.Log($"{player.name} gained Gold x1.");
                break;
        }
    }

    public void GiveStartTileBonus(PlayerController player)
    {
        player.inventory.Add(ResourceType.Gold, startGold);
        player.inventory.Add(ResourceType.Silver, startSilver);
        player.inventory.Add(ResourceType.Bronze, startBronze);

        Debug.Log($"{player.name} gained {startGold} Gold, {startSilver} Silver, {startBronze} Bronze for passing Start.");
    }
}
