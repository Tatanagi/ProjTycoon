using UnityEngine;

public static class Quarry
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null || player.isInDebt)
        {
            Debug.LogWarning($"{player?.name ?? "Player"} cannot gain resources from Quarry: {(player == null ? "Player is null" : player.inventory == null ? "Inventory is null" : "In debt due to unpaid loan")}");
            return;
        }

        player.inventory.Add(ResourceType.Bronze, 1);
        Debug.Log($"{player.name} gained 1 Bronze Token from Quarry.");
    }
}