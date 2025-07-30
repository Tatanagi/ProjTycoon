using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomEffectRounds : MonoBehaviour
{
    public static RandomEffectRounds Instance { get; private set; }

    // Enum to identify effects in the Inspector
    public enum EffectType
    {
        WrongCurrency,
        TurnipConversion
    }

    [System.Serializable]
    public struct EffectConfig
    {
        public EffectType effectType;
        [Range(0f, 100f)] // Weight as a percentage for easier configuration
        public float weight;
    }

    [SerializeField]
    private List<EffectConfig> effectConfigs = new List<EffectConfig>
    {
        new EffectConfig { effectType = EffectType.WrongCurrency, weight = 50f },
        new EffectConfig { effectType = EffectType.TurnipConversion, weight = 50f }
    };

    private List<Action> roundEffects;
    private Action currentEffect;
    private bool isEffectActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeEffects();
    }

    private void InitializeEffects()
    {
        roundEffects = new List<Action>();
        foreach (var config in effectConfigs)
        {
            switch (config.effectType)
            {
                case EffectType.WrongCurrency:
                    roundEffects.Add(WrongCurrencyEffect);
                    break;
                case EffectType.TurnipConversion:
                    roundEffects.Add(TurnipConversionEffect);
                    break;
                default:
                    Debug.LogWarning($"Unknown effect type: {config.effectType}");
                    break;
            }
        }

        // Validate weights
        float totalWeight = 0f;
        foreach (var config in effectConfigs)
        {
            totalWeight += config.weight;
        }
        if (Mathf.Approximately(totalWeight, 0f))
        {
            Debug.LogError("Total weight of effects is zero. Please assign positive weights in the Unity Inspector.");
        }
    }

    public void ApplyRandomEffectWithPanel(int round)
    {
        if (round <= 1)
        {
            return; // No effect in the first round
        }

        if (!isEffectActive)
        {
            currentEffect = SelectWeightedRandomEffect();
            if (currentEffect == null)
            {
                Debug.LogWarning("No effect selected. Check effect weights.");
                return;
            }
            isEffectActive = true;

            string title = "Round Effect!";
            string description = GetEffectDescription();

            Debug.Log("Showing CA Panel for Round Effect");
            UIManager.Instance.ShowCellAction(
                title,
                $"Round {round}: {description}",
                null,
                () =>
                {
                    currentEffect?.Invoke();
                    Debug.Log($"Round {round} effect applied: {description}");
                    isEffectActive = false;
                    // Update turn UI immediately after confirmation
                    TurnUIController.Instance.UpdateTurnUI();
                }
            );
        }
    }

    private Action SelectWeightedRandomEffect()
    {
        float totalWeight = 0f;
        foreach (var config in effectConfigs)
        {
            totalWeight += config.weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < effectConfigs.Count; i++)
        {
            cumulativeWeight += effectConfigs[i].weight;
            if (randomValue <= cumulativeWeight)
            {
                return roundEffects[i];
            }
        }

        Debug.LogWarning("Weighted random selection failed. Returning null.");
        return null;
    }

    private string GetEffectDescription()
    {
        if (currentEffect == WrongCurrencyEffect)
            return "A random resource (Bronze, Silver, or Gold) is worthless this round!";
        if (currentEffect == TurnipConversionEffect)
            return "Players can convert 5 Turnips to 1 Shiny Penny this round!";
        return "Unknown effect.";
    }

    private void WrongCurrencyEffect()
    {
        WrongCurrency wrongCurrency = FindFirstObjectByType<WrongCurrency>();
        if (wrongCurrency != null)
        {
            wrongCurrency.ActivateEffect();
        }
        else
        {
            Debug.LogWarning("WrongCurrency component not found!");
        }
    }

    private void TurnipConversionEffect()
    {
        TurnipCraze turnipCraze = FindFirstObjectByType<TurnipCraze>();
        if (turnipCraze != null)
        {
            turnipCraze.ActivateTurnipConversion();
            // Apply conversion to all players
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                turnipCraze.ConvertTurnipsToShinyPennies(player);
            }
        }
        else
        {
            Debug.LogWarning("TurnipCraze component not found!");
        }
    }

    public void ResetEffect()
    {
        if (isEffectActive)
        {
            if (currentEffect == WrongCurrencyEffect)
            {
                WrongCurrency wrongCurrency = FindFirstObjectByType<WrongCurrency>();
                if (wrongCurrency != null)
                {
                    wrongCurrency.ResetEffect();
                }
            }
            else if (currentEffect == TurnipConversionEffect)
            {
                TurnipCraze turnipCraze = FindFirstObjectByType<TurnipCraze>();
                if (turnipCraze != null)
                {
                    turnipCraze.DeactivateTurnipConversion();
                }
            }
            isEffectActive = false;
            currentEffect = null;
            Debug.Log("Round effect reset for the new round.");
        }
    }
}