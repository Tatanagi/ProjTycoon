using UnityEngine;

public static class WheatField
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null || player.isInDebt)
        {
            Debug.LogWarning($"{player?.name ?? "Player"} cannot gain resources from WheatField: {(player == null ? "Player is null" : player.inventory == null ? "Inventory is null" : "In debt due to unpaid loan")}");
            return;
        }

        int turnips = 5;
        if (TurnipCraze.Instance != null)
        {
            if (TurnipCraze.Instance.isTurnipConversionActive)
            {
                turnips = 10;
                Debug.Log($"{player.name} gained 10 turnips due to the Turnip Conversion Effect!");
            }
            else if (TurnipCraze.Instance.isCrazeActive)
            {
                turnips = 10;
                Debug.Log($"{player.name} gained 10 turnips due to the Turnip Craze!");
            }
        }

        player.inventory.Add(ResourceType.Turnips, turnips);
        Debug.Log($"{player.name} harvested at the Wheat Field: +{turnips} Turnips.");
    }
}