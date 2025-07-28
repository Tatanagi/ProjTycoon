using UnityEngine;

public static class MiningShaft
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null || player.isInDebt)
        {
            Debug.LogWarning($"{player?.name ?? "Player"} cannot gain resources from MiningShaft: {(player == null ? "Player is null" : player.inventory == null ? "Inventory is null" : "In debt due to unpaid loan")}");
            return;
        }

        player.inventory.Add(ResourceType.Gold, 1);
        Debug.Log($"{player.name} mined at the Shaft: +1 Gold Token.");
    }
}