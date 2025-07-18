using UnityEngine;

public static class WheatField
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        int turnips = 5;
        if (TurnipCraze.Instance != null && TurnipCraze.Instance.isCrazeActive)
        {
            turnips *= 2;
            Debug.Log($"{player.name} gained double turnips due to the Turnip Craze!");
        }

        player.inventory.Add(ResourceType.Turnips, turnips);

        Debug.Log($"{player.name} harvested at the Wheat Field: +{turnips} Turnips.");
    }
}