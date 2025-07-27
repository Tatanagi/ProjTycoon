using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomEffectRounds : MonoBehaviour
{
    public static RandomEffectRounds Instance { get; private set; }

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
        roundEffects = new List<Action>
        {
            WrongCurrencyEffect,
            TurnipConversionEffect // Added new turnip conversion effect
        };
    }

    public void ApplyRandomEffectWithPanel(int round)
    {
        if (round <= 1)
        {
            return; // No effect in the first round
        }

        if (!isEffectActive)
        {
            currentEffect = roundEffects[UnityEngine.Random.Range(0, roundEffects.Count)];
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