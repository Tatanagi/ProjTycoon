using UnityEngine;

public static class Fishery
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null || player.isInDebt)
        {
            Debug.LogWarning($"{player?.name ?? "Player"} cannot gain resources from Fishery: {(player == null ? "Player is null" : player.inventory == null ? "Inventory is null" : "In debt due to unpaid loan")}");
            return;
        }

        player.inventory.Add(ResourceType.Silver, 1);
        Debug.Log($"{player.name} visited the Fishery: +1 Silver Token.");
    }
}