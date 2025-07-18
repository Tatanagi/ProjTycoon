using UnityEngine;

public static class Stables
{
    public static void Execute(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.ShinyPennies, 1);

        ResourceType randomType = (ResourceType)Random.Range(0, 3); // 0, 1, 2 for Bronze, Silver, Gold
        switch (randomType)
        {
            case ResourceType.Bronze:
                player.inventory.Add(ResourceType.Bronze, 1);
                break;
            case ResourceType.Silver:
                player.inventory.Add(ResourceType.Silver, 1);
                break;
            case ResourceType.Gold:
                player.inventory.Add(ResourceType.Gold, 1);
                break;
        }

        Debug.Log($"{player.name} visited the Stables: +1 shiny penny and 1 random resource ({randomType})");
    }
}