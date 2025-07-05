using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Bronze, Silver, Gold }

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
                player.bronze += 2;
                Debug.Log($"{player.name} gained Bronze x2.");
                break;
            case <= 5: // Change to 5
                player.silver += 1;
                Debug.Log($"{player.name} gained Silver x1.");
                break;
            default:
                player.gold += 1;
                Debug.Log($"{player.name} gained Gold x1.");
                break;
        }
    }

    public void GiveStartTileBonus(PlayerController player)
    {
        player.gold += startGold;
        player.silver += startSilver;
        player.bronze += startBronze;

        Debug.Log($"{player.name} gained {startGold} Gold, {startSilver} Silver, {startBronze} Bronze for passing Start.");
    }
}
