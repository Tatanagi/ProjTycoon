using UnityEngine;
using System;

public enum CellType
{
    Normal,
    CommunityChest,
    LuckyLoanLender,
    RoyalMint,
    ResourceTokens,
    SuddenShortage,
    Stables,
    Quarry,
    Fishery,
    WheatField,
    MiningShaft,
    Thief
}

public class BoardCell : MonoBehaviour
{
    public CellType cellType = CellType.Normal;
    private TurnManager turnManager; // Store the TurnManager reference

    [Header("Audio")]
    [SerializeField] private AudioSource cellAudioSource; // AudioSource for cell landing SFX
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f; // Volume for cell landing SFX
    [SerializeField] private AudioClip normalClip; // SFX for Normal cell
    [SerializeField] private AudioClip communityChestClip; // SFX for CommunityChest cell
    [SerializeField] private AudioClip luckyLoanLenderClip; // SFX for LuckyLoanLender cell
    [SerializeField] private AudioClip royalMintClip; // SFX for RoyalMint cell
    [SerializeField] private AudioClip resourceTokensClip; // SFX for ResourceTokens cell
    [SerializeField] private AudioClip suddenShortageClip; // SFX for SuddenShortage cell
    [SerializeField] private AudioClip stablesClip; // SFX for Stables cell
    [SerializeField] private AudioClip quarryClip; // SFX for Quarry cell
    [SerializeField] private AudioClip fisheryClip; // SFX for Fishery cell
    [SerializeField] private AudioClip wheatFieldClip; // SFX for WheatField cell
    [SerializeField] private AudioClip miningShaftClip; // SFX for MiningShaft cell
    [SerializeField] private AudioClip thiefClip; // SFX for Thief cell

    void Awake()
    {
        // Initialize AudioSource
        if (cellAudioSource == null)
        {
            cellAudioSource = gameObject.AddComponent<AudioSource>();
            cellAudioSource.playOnAwake = false;
            cellAudioSource.loop = false;
            cellAudioSource.spatialBlend = 0f; // 2D sound for board game

            // Assign SFX mixer group
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                cellAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
            else
            {
                Debug.LogWarning($"{name} could not find AudioManager or AudioMixer. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
    }

    public void OnPlayerLanded(PlayerController player, TurnManager turnManager)
    {
        this.turnManager = turnManager; // Assign the passed TurnManager
        Debug.Log($"[BoardCell] Player {player.name} landed on {cellType}.");

        // Play cell-specific landing sound
        AudioClip clipToPlay = GetClipForCellType(cellType);
        if (cellAudioSource != null && clipToPlay != null)
        {
            cellAudioSource.PlayOneShot(clipToPlay, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"[BoardCell] Cannot play landing sound for {cellType}: AudioSource or AudioClip is missing.");
        }

        Action onConfirm = null;

        if (turnManager == null)
        {
            Debug.LogError("[BoardCell] TurnManager not assigned! Cannot proceed.");
            return;
        }

        // Prevent resource gains during Debt round for specific cells
        if (player.isInDebt &&
            (cellType == CellType.Stables || cellType == CellType.Quarry ||
             cellType == CellType.Fishery || cellType == CellType.WheatField ||
             cellType == CellType.MiningShaft))
        {
            UIManager.Instance.ShowCellAction(
                GetCellActionTitle(),
                "This round you will not gain currency due to unpaid loan!",
                player,
                () => { Dice.Instance?.OnActionConfirmed(); }
            );
            Debug.Log($"[BoardCell] {player.name} is in Debt Round: No resources gained from {cellType}.");
            return;
        }

        switch (cellType)
        {
            case CellType.CommunityChest:
                UIManager.Instance.ShowCommunityChestCard(player);
                break;
            case CellType.LuckyLoanLender:
                UIManager.Instance.ShowLoanOffer(player);
                break;
            case CellType.RoyalMint:
                UIManager.Instance.ShowExchange();
                break;
            case CellType.ResourceTokens:
                onConfirm = () => GameManager.Instance.GiveStartTileBonus(player);
                break;
            case CellType.SuddenShortage:
                onConfirm = () => SuddenShortage.Execute(player);
                break;
            case CellType.Stables:
                onConfirm = () => Stables.Execute(player);
                break;
            case CellType.Quarry:
                onConfirm = () => Quarry.Execute(player);
                break;
            case CellType.Fishery:
                onConfirm = () => Fishery.Execute(player);
                break;
            case CellType.WheatField:
                onConfirm = () => WheatField.Execute(player);
                break;
            case CellType.MiningShaft:
                onConfirm = () => MiningShaft.Execute(player);
                break;
            case CellType.Thief:
                onConfirm = () => Thief.Execute(player);
                break;
            case CellType.Normal:
                // Check if this is the last player of the round (index 3 for 4 players)
                if (turnManager.GetCurrentPlayerIndex() == turnManager.totalPlayers - 1)
                {
                    onConfirm = () =>
                    {
                        Debug.Log($"[BoardCell] Round finished - Applying round effect and advancing to Player 1");
                        turnManager.NextTurn(); // This will increment to 0 and start a new round
                        TurnUIController.Instance.UpdateTurnUI(); // Show Player 1's turn
                    };
                    UIManager.Instance.ShowCellAction(
                        "Round Effect",
                        "A random resource effect applies at the end of the round!",
                        player,
                        onConfirm
                    );
                }
                else
                {
                    // For non-last players, just update UI to continue the round
                    TurnUIController.Instance.UpdateTurnUI();
                }
                break;
        }

        if (onConfirm != null && cellType != CellType.Normal)
        {
            UIManager.Instance.ShowCellAction(
                GetCellActionTitle(),
                GetCellActionDescription(player),
                player,
                () =>
                {
                    onConfirm.Invoke();
                    Dice.Instance?.OnActionConfirmed();
                }
            );
        }
    }

    private AudioClip GetClipForCellType(CellType type)
    {
        switch (type)
        {
            case CellType.Normal: return normalClip;
            case CellType.CommunityChest: return communityChestClip;
            case CellType.LuckyLoanLender: return luckyLoanLenderClip;
            case CellType.RoyalMint: return royalMintClip;
            case CellType.ResourceTokens: return resourceTokensClip;
            case CellType.SuddenShortage: return suddenShortageClip;
            case CellType.Stables: return stablesClip;
            case CellType.Quarry: return quarryClip;
            case CellType.Fishery: return fisheryClip;
            case CellType.WheatField: return wheatFieldClip;
            case CellType.MiningShaft: return miningShaftClip;
            case CellType.Thief: return thiefClip;
            default: return null;
        }
    }

    public string GetCellActionTitle()
    {
        return cellType.ToString();
    }

    public string GetCellActionDescription(PlayerController currentPlayer)
    {
        if (currentPlayer == null) return "";

        if (currentPlayer.isInDebt &&
            (cellType == CellType.Stables || cellType == CellType.Quarry ||
             cellType == CellType.Fishery || cellType == CellType.WheatField ||
             cellType == CellType.MiningShaft))
        {
            return "This round you will not gain currency due to unpaid loan!";
        }

        if (turnManager == null)
        {
            Debug.LogWarning("[BoardCell] turnManager is null in GetCellActionDescription");
            return "";
        }

        switch (cellType)
        {
            case CellType.Stables:
                return "Gain 1 Shiny Penny and 1 random resource!";
            case CellType.Quarry:
                return "Gain 1 Bronze Token!";
            case CellType.Fishery:
                return "Gain 1 Silver Token!";
            case CellType.WheatField:
                bool isTurnipConversionActive = TurnipCraze.Instance != null && TurnipCraze.Instance.isTurnipConversionActive;
                bool isCrazeActive = TurnipCraze.Instance != null && TurnipCraze.Instance.isCrazeActive;
                int turnipsGained = isTurnipConversionActive || isCrazeActive ? 10 : 5;
                string description = $"Gain {turnipsGained} Turnips!";
                if (isTurnipConversionActive)
                    description += " (Increased during Turnip Conversion Effect)";
                else if (isCrazeActive)
                    description += " (Increased during Turnip Craze)";
                return description;
            case CellType.MiningShaft:
                return "Gain 1 Gold Token!";
            case CellType.Thief:
                PlayerController[] allPlayers = GameManager.Instance.GetAllPlayers();
                if (allPlayers == null || allPlayers.Length == 0) return "Thief: No players available!";
                int currentIndex = Array.IndexOf(allPlayers, currentPlayer);
                int targetIndex = (currentIndex + 1) % allPlayers.Length;
                PlayerController targetPlayer = allPlayers[targetIndex];
                return $"{targetPlayer.name}: Lose up to 20% Bronze, 10% Silver, 5% Gold to a thief!";
            case CellType.ResourceTokens:
                return "Confirm to receive +5 gold, +5 silver, and +5 bronze!";
            case CellType.SuddenShortage:
                return "In this round, all resource tokens will be capped to 20 for all players! This can only activated once.";
            case CellType.LuckyLoanLender:
                return "Would you like a loan worth 10% of your shiny pennies?";
            case CellType.Normal:
                if (turnManager.GetCurrentPlayerIndex() == turnManager.totalPlayers - 1)
                {
                    return "A random resource effect applies at the end of the round!";
                }
                return "";
            default:
                return "";
        }
    }
}