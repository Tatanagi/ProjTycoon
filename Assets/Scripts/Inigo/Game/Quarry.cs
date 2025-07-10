using UnityEngine;

public static class Quarry
{
    public static void Execute(PlayerController player)
    {
        player.inventory.Bronze += 1;

        Debug.Log($"{player.name} mined at the Quarry: +1 Bronze Token.");
        UIManager.Instance.ShowCellAction
        (
            "Quarry",
            $"{player.name} gained 1 Bronze Token!",
            player
        );
    }
}
