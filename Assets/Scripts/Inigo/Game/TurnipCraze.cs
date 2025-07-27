using UnityEngine;

public class TurnipCraze : MonoBehaviour
{
    public static TurnipCraze Instance { get; private set; }

    [Header("Craze Settings")]
    public bool isCrazeActive = false;
    public float turnipValueMultiplier = 1.5f;
    public bool isTurnipConversionActive = false; // New flag for turnip conversion effect
    private const int turnipsRequiredForConversion = 5; // Number of turnips needed for 1 shiny penny
    private const int shinyPenniesGained = 1; // Shiny pennies gained per conversion

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure persistence across scenes
        }
    }

    /// <summary>
    /// Call this to activate Turnip Craze for the round (value multiplier).
    /// </summary>
    public void ActivateCraze()
    {
        isCrazeActive = true;
        Debug.Log("The Great Turnip Craze is active! Turnip Tokens are worth 50% more!");
    }

    /// <summary>
    /// Call this to end Turnip Craze for the round (value multiplier).
    /// </summary>
    public void DeactivateCraze()
    {
        isCrazeActive = false;
        Debug.Log("The Great Turnip Craze has ended.");
    }

    /// <summary>
    /// Gets the value of turnips, factoring in the craze multiplier if active.
    /// </summary>
    public int GetTurnipValue(int baseAmount)
    {
        return isCrazeActive ? Mathf.CeilToInt(baseAmount * turnipValueMultiplier) : baseAmount;
    }

    /// <summary>
    /// Activates the turnip-to-shiny-penny conversion effect for the round.
    /// </summary>
    public void ActivateTurnipConversion()
    {
        isTurnipConversionActive = true;
        Debug.Log("Turnip Conversion Effect activated! Players can convert 5 Turnips to 1 Shiny Penny.");
    }

    /// <summary>
    /// Deactivates the turnip-to-shiny-penny conversion effect.
    /// </summary>
    public void DeactivateTurnipConversion()
    {
        isTurnipConversionActive = false;
        Debug.Log("Turnip Conversion Effect has ended.");
    }

    /// <summary>
    /// Converts 5 turnips to 1 shiny penny for the specified player if conversion is active.
    /// </summary>
    public bool ConvertTurnipsToShinyPennies(PlayerController player)
    {
        if (!isTurnipConversionActive)
        {
            Debug.Log("Turnip Conversion Effect is not active.");
            return false;
        }

        if (player == null || player.inventory == null)
        {
            Debug.LogWarning("Player or player inventory is null.");
            return false;
        }

        var inventory = player.inventory;
        if (inventory.CanAfford(ResourceType.Turnips, turnipsRequiredForConversion))
        {
            inventory.Spend(ResourceType.Turnips, turnipsRequiredForConversion);
            inventory.Add(ResourceType.ShinyPennies, shinyPenniesGained);
            Debug.Log($"{player.name} converted {turnipsRequiredForConversion} Turnips to {shinyPenniesGained} Shiny Penny.");
            return true;
        }
        else
        {
            Debug.LogWarning($"{player.name} does not have enough Turnips ({inventory.TurnipsValue}/{turnipsRequiredForConversion}) to convert to a Shiny Penny.");
            return false;
        }
    }
}