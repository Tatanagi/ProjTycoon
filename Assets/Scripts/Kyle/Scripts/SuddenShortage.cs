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

    public bool IsActive => isActive;
    public int TriggeredRound => triggeredRound;

    public void TryTriggerShortage(PlayerController triggeringPlayer)
    {
        if (isActive || shortageRoundsTriggered >= maxShortageRounds || GameManager.Instance.turnManager.GetCurrentRound() == triggeredRound)
            return;

        TriggerShortage(triggeringPlayer);
    }

    private void TriggerShortage(PlayerController triggeringPlayer)
    {
        affectedPlayers = new List<PlayerController>(GameManager.Instance.GetAllPlayers());
        triggeredRound = GameManager.Instance.turnManager.GetCurrentRound();
        shortageRoundsTriggered++;
        isActive = true;

        Debug.Log("Sudden Shortage triggered! Limiting all players' gold/silver/bronze to 20.");

        foreach (var player in affectedPlayers)
        {
            SetPlayerShortage(player, true);
            ClampPlayerResources(player);
            player.inventory.OnChanged += () => EnforceClamp(player);
        }

        StartCoroutine(WaitForRoundEnd());
    }

    private void SetPlayerShortage(PlayerController player, bool state)
    {
        if (player != null && player.inventory != null)
        {
            player.inventory.SetShortage(state); // Use SetShortage method
            player.inventory.Notify(); // Redundant with SetShortage's OnChanged, but kept for clarity
        }
    }

    private void ClampPlayerResources(PlayerController player)
    {
        var inv = player.inventory;
        if (inv == null) return;

        int excessGold = Mathf.Max(0, inv.GoldValue - 20);
        int excessSilver = Mathf.Max(0, inv.SilverValue - 20);
        int excessBronze = Mathf.Max(0, inv.BronzeValue - 20);

        if (excessGold > 0) inv.Add(ResourceType.Gold, -excessGold);
        if (excessSilver > 0) inv.Add(ResourceType.Silver, -excessSilver);
        if (excessBronze > 0) inv.Add(ResourceType.Bronze, -excessBronze);
    }

    private void EnforceClamp(PlayerController player)
    {
        if (!player.inventory.IsInShortage) return;

        var inv = player.inventory;
        if (inv == null) return;

        int excessGold = Mathf.Max(0, inv.GoldValue - 20);
        int excessSilver = Mathf.Max(0, inv.SilverValue - 20);
        int excessBronze = Mathf.Max(0, inv.BronzeValue - 20);

        if (excessGold > 0) inv.Add(ResourceType.Gold, -excessGold);
        if (excessSilver > 0) inv.Add(ResourceType.Silver, -excessSilver);
        if (excessBronze > 0) inv.Add(ResourceType.Bronze, -excessBronze);
    }

    private IEnumerator WaitForRoundEnd()
    {
        int startRound = GameManager.Instance.turnManager.GetCurrentRound();

        while (GameManager.Instance.turnManager.GetCurrentRound() == startRound)
        {
            yield return null;
        }

        foreach (var player in affectedPlayers)
        {
            SetPlayerShortage(player, false);
            player.inventory.OnChanged -= () => EnforceClamp(player);
        }

        Debug.Log("Sudden Shortage ended. Resource caps lifted.");
        affectedPlayers.Clear();
        isActive = false;
    }
}