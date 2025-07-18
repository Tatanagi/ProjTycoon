using UnityEngine;

public static class MiningShaft
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.Gold, 1);

        Debug.Log($"{player.name} mined at the Shaft: +1 Gold Token.");
    }
}