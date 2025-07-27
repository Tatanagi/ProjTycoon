using UnityEngine;

public static class WheatField
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null)
        {
            Debug.LogWarning("Player or player inventory is null.");
            return;
        }

        int turnips = 5;
        if (TurnipCraze.Instance != null)
        {
            if (TurnipCraze.Instance.isTurnipConversionActive)
            {
                turnips = 10; // Set to 10 turnips during TurnipConversionEffect
                Debug.Log($"{player.name} gained 10 turnips due to the Turnip Conversion Effect!");
            }
            else if (TurnipCraze.Instance.isCrazeActive)
            {
                turnips = 10; // Set to 10 turnips during TurnipCraze
                Debug.Log($"{player.name} gained 10 turnips due to the Turnip Craze!");
            }
        }

        player.inventory.Add(ResourceType.Turnips, turnips);
        Debug.Log($"{player.name} harvested at the Wheat Field: +{turnips} Turnips.");
    }
}