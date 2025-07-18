using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenShortage : MonoBehaviour
{
    [Header("Settings")]
    public int maxShortageRounds = 1;

    private int shortageRoundsTriggered = 0;
    private bool isActive = false;
    private int triggeredRound = -1;
    private List<PlayerController> affectedPlayers = new();

    // Public properties to access private fields
    public bool IsActive => isActive;
    public int TriggeredRound => triggeredRound;

    public void TryTriggerShortage(PlayerController triggeringPlayer)
    {
        if (isActive ||
            shortageRoundsTriggered >= maxShortageRounds ||
            GameManager.Instance.turnManager.GetCurrentRound() == triggeredRound)
            return;

        TriggerShortage(triggeringPlayer);
    }

    private void TriggerShortage(PlayerController triggeringPlayer)
    {
        affectedPlayers = GameManager.Instance.GetAllPlayers();
        triggeredRound = GameManager.Instance.turnManager.GetCurrentRound();
        shortageRoundsTriggered++;
        isActive = true;

        Debug.Log("Sudden Shortage triggered! Limiting all players' gold/silver/bronze to 20.");

        foreach (var player in affectedPlayers)
        {
            player.inventory.isInShortage = true;
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
        if (!player.inventory.isInShortage) return;

        var inv = player.inventory;

        if (inv.Gold > 20) inv.Add(ResourceType.Gold, 20 - inv.Gold);
        if (inv.Silver > 20) inv.Add(ResourceType.Silver, 20 - inv.Silver);
        if (inv.Bronze > 20) inv.Add(ResourceType.Bronze, 20 - inv.Bronze);
    }

    private IEnumerator WaitForRoundEnd()
    {
        int startRound = GameManager.Instance.turnManager.GetCurrentRound();

        // Wait until a new round begins
        while (GameManager.Instance.turnManager.GetCurrentRound() == startRound)
        {
            yield return null;
        }

        // Remove effect
        foreach (var player in affectedPlayers)
        {
            player.inventory.isInShortage = false;
            player.inventory.OnChanged -= () => EnforceClamp(player);
        }

        Debug.Log("Sudden Shortage ended. Resource caps lifted.");
        affectedPlayers.Clear();
        isActive = false;
    }
}