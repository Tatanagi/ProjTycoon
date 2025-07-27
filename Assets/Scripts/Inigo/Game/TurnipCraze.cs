using UnityEngine;

public class TurnipCraze : MonoBehaviour
{
    public static TurnipCraze Instance { get; private set; }

    [Header("Craze Settings")]
    public bool isCrazeActive = false;
    public float turnipValueMultiplier = 1.5f;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Call this to activate Turnip Craze for the round.
    /// </summary>
    public void ActivateCraze()
    {
        isCrazeActive = true;
        Debug.Log("The Great Turnip Craze is active! Turnip Tokens are worth 50% more!");
    }

    /// <summary>
    /// Call this to end Turnip Craze for the round.
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
}
