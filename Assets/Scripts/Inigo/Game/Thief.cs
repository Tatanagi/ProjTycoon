using System;
using UnityEngine;

public static class Thief
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        PlayerController[] allPlayers = GameManager.Instance.GetAllPlayers();
        if (allPlayers == null || allPlayers.Length <= 1) return;

        int currentIndex = Array.IndexOf(allPlayers, player);
        int targetIndex = (currentIndex + 1) % allPlayers.Length;
        PlayerController target = allPlayers[targetIndex];
        if (target == null || target.inventory == null) return;

        int stolenBronze = Mathf.FloorToInt(target.inventory.BronzeValue * 0.20f);
        int stolenSilver = Mathf.FloorToInt(target.inventory.SilverValue * 0.10f);
        int stolenGold = Mathf.FloorToInt(target.inventory.GoldValue * 0.05f);

        stolenBronze = Mathf.Min(stolenBronze, target.inventory.BronzeValue);
        stolenSilver = Mathf.Min(stolenSilver, target.inventory.SilverValue);
        stolenGold = Mathf.Min(stolenGold, target.inventory.GoldValue);

        if (target.inventory.Spend(ResourceType.Bronze, stolenBronze) &&
            target.inventory.Spend(ResourceType.Silver, stolenSilver) &&
            target.inventory.Spend(ResourceType.Gold, stolenGold))
        {
            player.inventory.Add(ResourceType.Bronze, stolenBronze);
            player.inventory.Add(ResourceType.Silver, stolenSilver);
            player.inventory.Add(ResourceType.Gold, stolenGold);

            Debug.Log($"{player.name} stole {stolenBronze} Bronze, {stolenSilver} Silver, {stolenGold} Gold from {target.name}!");
        }
        else
        {
            Debug.LogWarning("Thief failed to steal due to insufficient resources.");
        }
    }
}