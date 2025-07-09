using UnityEngine;

public static class Fishery
{
    public static void Execute(PlayerController player)
    {
        player.inventory.Silver += 1;

        Debug.Log($"{player.name} visited the Fishery: +1 Silver Token.");
        UIManager.Instance.ShowNotification($"{player.name} gained 1 Silver Token!");
    }
}
