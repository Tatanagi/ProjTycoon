using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StickyFingers : MonoBehaviour
{
    [Header("This Player's Info")]
    public string playerTag;

    [Header("Sticky Fingers UI Components")]
    public GameObject stickyFingersPanel; // UI panel of Sticky Fingers
    public TMP_Dropdown targetDropdown; // Dropdown to choose which player to steal from
    public TMP_Dropdown resourceTypeDropdown; // Dropdown to choose what coin type to steal
    public TMP_InputField amountInput; // Input field to enter how many coins to steal

    [Header("Cooldown Settings")]
    [SerializeField] private int cooldownRounds = 4; // Cooldown/number of rounds before Sticky Fingers can be reused
    [SerializeField] private TurnManager turnManager; // Reference to TurnManager, assigned in Inspector

    private PlayerInventory inventory; // Current player's inventory
    private int lastUsedRound = -4; // Round when Sticky Fingers was last used (initialized to allow immediate use)

    // Shared list of all players' StickyFingers scripts (helps in detecting active player)
    public static List<StickyFingers> allStickyFingers = new List<StickyFingers>();

    private void Awake()
    {
        // Registers this player to the global list
        allStickyFingers.Add(this);

        // Validate TurnManager assignment
        if (turnManager == null)
        {
            Debug.LogError($"{playerTag}'s StickyFingers: TurnManager is not assigned in the Inspector!");
        }
    }

    private void OnDestroy()
    {
        // Unregister when destroyed (clean up)
        allStickyFingers.Remove(this);
    }

    private void Start()
    {
        // Gets current player's inventory, and hide canvas/UI by default
        inventory = GetComponent<PlayerInventory>();
        stickyFingersPanel.SetActive(false);
    }

    // Finds the StickyFingers script of the current player based on TurnManager index
    public static StickyFingers GetStickyFingersByPlayerIndex(int index)
    {
        string tagName = "Player" + (index + 1);
        foreach (var sf in allStickyFingers)
        {
            if (sf.playerTag == tagName)
                return sf;
        }
        return null;
    }

    // Called when the "Sticky Fingers" button is pressed
    public void ActivateStickyFingersUI()
    {
        if (!CanUseStickyFingers())
        {
            Debug.LogWarning($"{playerTag}'s Sticky Fingers is on cooldown. Can be used again in round {lastUsedRound + cooldownRounds + 1}.");
            return;
        }

        PopulateTargetDropdown(); // Fill dropdown excluding current player
        stickyFingersPanel.SetActive(true); // Show canvas/UI
    }

    // Hides the Sticky Fingers canvas/UI
    public void CloseStickyFingersUI()
    {
        stickyFingersPanel.SetActive(false);
    }

    // Called when confirm button is pressed
    public void OnStickyFingersConfirmed()
    {
        // Get targeted player and coin type
        string targetTag = targetDropdown.options[targetDropdown.value].text;
        string resourceStr = resourceTypeDropdown.options[resourceTypeDropdown.value].text;

        // Parse amount to steal
        if (!int.TryParse(amountInput.text, out int amount))
        {
            Debug.LogWarning("Invalid amount entered.");
            return;
        }

        // Parse resource type
        if (!Enum.TryParse(resourceStr, out ResourceType type))
        {
            Debug.LogWarning("Invalid resource type selected.");
            return;
        }

        // Attempt to steal
        bool success = TrySteal(targetTag, type, amount);

        if (success)
        {
            Debug.Log($"{playerTag} stole {amount} {type} from {targetTag}.");
            CloseStickyFingersUI();
        }
        else
        {
            Debug.Log("Steal attempt failed.");
        }
    }

    // Populate the dropdown with other player tags, excluding the current player
    private void PopulateTargetDropdown()
    {
        List<string> allPlayerTags = new List<string> { "Player1", "Player2", "Player3", "Player4" };
        allPlayerTags.Remove(playerTag);

        targetDropdown.ClearOptions();
        targetDropdown.AddOptions(allPlayerTags);
    }

    // MAIN LOGIC TO PERFORM STEALING
    public bool TrySteal(string targetTag, ResourceType type, int amount)
    {
        // Prevent stealing from self
        if (targetTag == playerTag)
        {
            Debug.LogWarning("Can't steal from self.");
            return false;
        }

        // Validate input amount is in limit range
        if (!IsValidAmount(type, amount))
        {
            Debug.LogWarning("Amount exceeds allowed limits.");
            return false;
        }

        // Find target player using player tags
        GameObject targetPlayer = GameObject.FindGameObjectWithTag(targetTag);
        if (targetPlayer == null) return false;

        PlayerInventory targetInventory = targetPlayer.GetComponent<PlayerInventory>();
        if (targetInventory == null) return false;

        // Makes sure the target has enough coins to be stolen
        int targetAmount = GetResourceAmount(targetInventory, type);
        if (targetAmount < amount)
        {
            Debug.LogWarning($"{targetTag} only has {targetAmount} {type}, can't steal {amount}.");
            return false;
        }

        // Transfers stolen coins to player's PlayerInventory
        targetInventory.Spend(type, amount);
        inventory.Add(type, amount);
        // Update last used round
        if (turnManager != null)
        {
            lastUsedRound = turnManager.GetCurrentRound();
        }
        else
        {
            Debug.LogError($"{playerTag}'s StickyFingers: Cannot update cooldown because TurnManager is null.");
            return false; // Prevent successful steal if TurnManager is missing
        }

        return true;
    }

    // Called by turn system when a new round begins
    public void OnRoundStart()
    {
        // No need to increment anything; CanUseStickyFingers checks round difference
    }

    // Checks if the ability can be used this round
    public bool CanUseStickyFingers()
    {
        if (turnManager == null)
        {
            Debug.LogError($"{playerTag}'s StickyFingers: Cannot check cooldown because TurnManager is null.");
            return false;
        }
        int currentRound = turnManager.GetCurrentRound();
        return (currentRound - lastUsedRound) >= cooldownRounds;
    }

    // Validates amount based on coin type's limits
    private bool IsValidAmount(ResourceType type, int amount)
    {
        return type switch
        {
            ResourceType.Bronze => amount >= 1 && amount <= 10,
            ResourceType.Silver => amount >= 1 && amount <= 5,
            ResourceType.Gold => amount >= 1 && amount <= 2,
            _ => false
        };
    }

    // Gets the amount of resource coins in a player's inventory
    private int GetResourceAmount(PlayerInventory inv, ResourceType type)
    {
        return type switch
        {
            ResourceType.Gold => inv.GoldValue,
            ResourceType.Silver => inv.SilverValue,
            ResourceType.Bronze => inv.BronzeValue,
            _ => 0
        };
    }
}