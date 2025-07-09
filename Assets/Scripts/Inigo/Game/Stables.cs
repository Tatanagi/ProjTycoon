using UnityEngine;

public static class Stables
{
    public static void Execute(PlayerController player)
    {
        player.inventory.ShinyPennies += 1;

        ResourceType randomType = (ResourceType)Random.Range(0, 3);
        switch (randomType)
        {
            case ResourceType.Bronze:
                player.inventory.Bronze += 1;
                break;
            case ResourceType.Silver:
                player.inventory.Silver += 1;
                break;
            case ResourceType.Gold:
                player.inventory.Gold += 1;
                break;
        }

        Debug.Log($"{player.name} visited the Stables: +1 shiny penny and 1 random resource ({randomType})");
        UIManager.Instance.ShowNotification($"{player.name} gained 1 shiny penny and 1 {randomType}!");
    }
}
