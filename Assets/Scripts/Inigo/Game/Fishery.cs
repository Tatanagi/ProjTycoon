using UnityEngine;

public static class Fishery
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.Silver, 1);

        Debug.Log($"{player.name} visited the Fishery: +1 Silver Token.");
    }
}