using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenShortage : MonoBehaviour
{
    [Header("Settings")]
    public int maxShortageRounds = 3;

    private int shortageRoundsTriggered = 0;
    private bool isActive = false;
    private List<PlayerController> affectedPlayers = new();

    public void TryTriggerShortage()
    {
        if (isActive || shortageRoundsTriggered >= maxShortageRounds) return;

        TriggerShortageForAll();
    }

    public void TriggerShortageForAll()
    {
        affectedPlayers = GameManager.Instance.GetAllPlayers();

        shortageRoundsTriggered++;
        isActive = true;

        Debug.Log("Sudden Shortage triggered! Limiting all players' gold/silver/bronze to 20.");

        foreach (var player in affectedPlayers)
        {
            ClampPlayerResources(player);
            player.inventory.OnChanged += () => EnforceClamp(player);
        }

        StartCoroutine(WaitForRoundEnd());
    }

    private void ClampPlayerResources(PlayerController player)
    {
        var inv = player.inventory;

        if (inv.Gold > 20) inv.Add(ResourceType.Gold, 20 - inv.Gold);
        if (inv.Silver > 20) inv.Add(ResourceType.Silver, 20 - inv.Silver);
        if (inv.Bronze > 20) inv.Add(ResourceType.Bronze, 20 - inv.Bronze);
    }

    private void EnforceClamp(PlayerController player)
    {
        var inv = player.inventory;

        if (inv.Gold > 20) inv.Add(ResourceType.Gold, 20 - inv.Gold);
        if (inv.Silver > 20) inv.Add(ResourceType.Silver, 20 - inv.Silver);
        if (inv.Bronze > 20) inv.Add(ResourceType.Bronze, 20 - inv.Bronze);
    }

    private IEnumerator WaitForRoundEnd()
    {
        int totalTurns = affectedPlayers.Count;
        int startIndex = GameManager.Instance.turnManager.GetCurrentPlayerIndex();
        int turnsPassed = 0;
        int lastSeenPlayer = startIndex;

        while (turnsPassed < totalTurns)
        {
            int current = GameManager.Instance.turnManager.GetCurrentPlayerIndex();
            if (current != lastSeenPlayer)
            {
                lastSeenPlayer = current;
                turnsPassed++;
            }
            yield return null;
        }

        // Remove effect
        foreach (var player in affectedPlayers)
        {
            player.inventory.OnChanged -= () => EnforceClamp(player);
        }

        Debug.Log("Sudden Shortage ended. Resource caps lifted.");
        affectedPlayers.Clear();
        isActive = false;
    }
}
