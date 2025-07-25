using UnityEngine;

public static class Quarry
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.Bronze, 1);
        Debug.Log($"{player.name} gained 1 Bronze Token from Quarry.");
    }
}