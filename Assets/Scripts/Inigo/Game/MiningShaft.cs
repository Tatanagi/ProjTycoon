using UnityEngine;

public static class MiningShaft
{
    public static void Execute(PlayerController player)
    {
        player.inventory.Gold += 1;

        Debug.Log($"{player.name} mined at the Shaft: +1 Gold Token.");
        UIManager.Instance.ShowCellAction
        (
            "Mining Shaft",
            $"{player.name} gained 1 Gold Token!",
            player
        );
    }
}
