using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Panels")]
    public GameObject CAPanel;
    public GameObject CCPanel;
    public GameObject CCPanelCards;
    public GameObject LOPanel;
    public GameObject REPanel;
    public GameObject LOPanelPayment;
    [Header("Cell Action Panel")]
    public TextMeshProUGUI cellTitleText;
    public TextMeshProUGUI cellDescriptionText;
    public Button cellConfirmButton;
    [Header("Community Chest")]
    public TextMeshProUGUI cardTitleText;
    public TextMeshProUGUI cardDescriptionText;
    public Button drawCardButton;
    [Header("Community Chest Card")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionCardText;
    public TextMeshProUGUI cardCostText;
    public Button yesButton;
    public Button noButton;
    public Image cardImage;
    [Header("Lucky Loan Lender")]
    public TextMeshProUGUI loanTitleText;
    public TextMeshProUGUI loanDescriptionText;
    public TMP_InputField loanAmountInput;
    public Button acceptLoanButton;
    public Button cancelLoanButton;
    public TextMeshProUGUI paymentTitleText;
    public TextMeshProUGUI paymentDescriptionText;
    public TMP_InputField paymentAmountInput;
    public Button confirmPaymentButton;
    public Button cancelPaymentButton;
    [Header("Royal Fickle Mint")]
    public TextMeshProUGUI exchangeTitleText;
    public TextMeshProUGUI bronzeInput;
    public TextMeshProUGUI silverInput;
    public TextMeshProUGUI goldInput;
    public Button confirmExchangeButton;
    public Button cancelExchangeButton;
    [Header("Turn UI Controller")]
    public TurnUIController turnController;
    [Header("Fields")]
    public TurnManager TurnManager;
    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonClickClip; // For general buttons
    [SerializeField] private AudioClip drawCardClip; // SFX for DrawCard
    [SerializeField] private AudioClip confirmBuyClip; // SFX for ConfirmBuy
    [SerializeField] private AudioClip cancelBuyClip; // SFX for CancelBuy
    [SerializeField][Range(0f, 1f)] private float buttonSfxVolume = 0.5f;
    private PlayerController currentPlayer;
    private PlayerController popupPlayer;
    private PlayerController mintingPlayer;
    private CommunityChest.CommunityCard _currentCard;
    private List<PlayerController> _otherPlayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (transform.parent != null)
        {
            transform.SetParent(null);
            Debug.Log("UIManager GameObject moved to root to support DontDestroyOnLoad.");
        }
        DontDestroyOnLoad(gameObject);
        if (buttonAudioSource == null)
        {
            buttonAudioSource = gameObject.AddComponent<AudioSource>();
            buttonAudioSource.playOnAwake = false;
            buttonAudioSource.loop = false;
            buttonAudioSource.spatialBlend = 0f;
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                buttonAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
            else
            {
                Debug.LogWarning("UIManager could not find AudioManager or AudioMixer. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
    }

    private void Start()
    {
        if (CAPanel != null) CAPanel.SetActive(false);
        else Debug.LogWarning("CAPanel is not assigned in UIManager!");
        if (CCPanel != null) CCPanel.SetActive(false);
        else Debug.LogWarning("CCPanel is not assigned in UIManager!");
        if (CCPanelCards != null) CCPanelCards.SetActive(false);
        else Debug.LogWarning("CCPanelCards is not assigned in UIManager!");
        if (LOPanel != null) LOPanel.SetActive(false);
        else Debug.LogWarning("LOPanel is not assigned in UIManager!");
        if (LOPanelPayment != null) LOPanelPayment.SetActive(false);
        else Debug.LogWarning("LOPanelPayment is not assigned in UIManager!");
        if (REPanel != null) REPanel.SetActive(false);
        else Debug.LogWarning("REPanel is not assigned in UIManager!");
        if (turnController == null)
        {
            turnController = FindFirstObjectByType<TurnUIController>();
            if (turnController == null)
                Debug.LogWarning("TurnUIController not found in scene!");
        }
        SetupLoanInput();
        SetupPaymentInput();
    }

    private void SetupLoanInput()
    {
        if (loanAmountInput != null)
        {
            loanAmountInput.onValueChanged.AddListener(OnLoanInputChanged);
            loanAmountInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
            loanAmountInput.characterLimit = 1;
        }
        else Debug.LogWarning("loanAmountInput is not assigned in UIManager!");
    }

    private void SetupPaymentInput()
    {
        if (paymentAmountInput != null)
        {
            paymentAmountInput.onValueChanged.AddListener(OnPaymentInputChanged);
            paymentAmountInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        }
        else Debug.LogWarning("paymentAmountInput is not assigned in UIManager!");
    }

    private void OnLoanInputChanged(string input)
    {
        if (int.TryParse(input, out int amount))
        {
            acceptLoanButton.interactable = (amount >= 1 && amount <= 5);
        }
        else
        {
            acceptLoanButton.interactable = false;
        }
    }

    private void OnPaymentInputChanged(string input)
    {
        if (currentPlayer == null) return;
        int debtAmount = currentPlayer.loanAmount * 2;
        if (int.TryParse(input, out int amount))
        {
            confirmPaymentButton.interactable = amount >= debtAmount && currentPlayer.inventory.CanAfford(ResourceType.ShinyPennies, amount);
        }
        else
        {
            confirmPaymentButton.interactable = false;
        }
    }

    public void ShowCellAction(string title, string description, PlayerController player = null, Action onConfirm = null)
    {
        popupPlayer = player;
        if (CAPanel == null)
        {
            Debug.LogWarning("CAPanel is not assigned in UIManager!");
            return;
        }
        if (cellTitleText != null) cellTitleText.text = title;
        else Debug.LogWarning("cellTitleText is not assigned in UIManager!");
        if (cellDescriptionText != null) cellDescriptionText.text = description;
        else Debug.LogWarning("cellDescriptionText is not assigned in UIManager!");
        if (cellConfirmButton != null)
        {
            cellConfirmButton.onClick.RemoveAllListeners();
            cellConfirmButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                onConfirm?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HideCellAction();
            });
        }
        else
        {
            Debug.LogWarning("cellConfirmButton is not assigned in UIManager!");
            StartCoroutine(HideCellActionAfterDelay(5f, onConfirm));
        }
        CAPanel.SetActive(true);
    }

    private IEnumerator HideCellActionAfterDelay(float delay, Action onConfirm = null)
    {
        yield return new WaitForSeconds(delay);
        onConfirm?.Invoke();
        if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
        HideCellAction();
    }

    public void HideCellAction()
    {
        if (CAPanel != null) CAPanel.SetActive(false);
    }

    public void ShowCommunityChestCard(PlayerController player)
    {
        if (CCPanel == null)
        {
            Debug.LogWarning("CCPanel is not assigned in UIManager!");
            return;
        }
        currentPlayer = player;
        if (cardTitleText != null) cardTitleText.text = "Community Chest";
        else Debug.LogWarning("cardTitleText is not assigned in UIManager!");
        if (cardDescriptionText != null) cardDescriptionText.text = player != null ? $"{player.name}: Click to draw a random effect!" : "Click to draw a random effect!";
        else Debug.LogWarning("cardDescriptionText is not assigned in UIManager!");
        CCPanel.SetActive(true);
    }

    [ContextMenu("Draw Card")]
    public void DrawCard()
    {
        if (CCPanelCards == null)
        {
            Debug.LogWarning("CCPanelCards is not assigned in UIManager!");
            return;
        }

        if (currentPlayer == null)
        {
            Debug.LogWarning("No current player assigned!");
            return;
        }

        CommunityChest chest = FindFirstObjectByType<CommunityChest>();
        if (chest == null)
        {
            Debug.LogWarning("CommunityChest not found!");
            return;
        }

        if (buttonAudioSource != null && drawCardClip != null)
        {
            buttonAudioSource.PlayOneShot(drawCardClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play DrawCard SFX: AudioSource or drawCardClip is missing.");
        }

        _otherPlayers = new List<PlayerController>(GameManager.Instance.GetAllPlayers()).FindAll(p => p != currentPlayer);
        _currentCard = chest._deck[UnityEngine.Random.Range(0, chest._deck.Count)];

        if (cardNameText != null) cardNameText.text = _currentCard.cardName;
        if (cardDescriptionCardText != null) cardDescriptionCardText.text = _currentCard.description;
        if (cardCostText != null) cardCostText.text = $"Cost: {_currentCard.cost} Shiny Pennies";
        if (cardImage != null)
        {
            cardImage.sprite = _currentCard.cardImage; // Set the card's sprite
            if (_currentCard.cardImage == null)
                Debug.LogWarning($"No sprite assigned for card: {_currentCard.cardName}");
        }
        else Debug.LogWarning("cardImage is not assigned in UIManager!");

        if (yesButton != null)
        {
            yesButton.interactable = currentPlayer.inventory.CanAfford(ResourceType.ShinyPennies, _currentCard.cost);
        }
        else
        {
            Debug.LogWarning("yesButton is not assigned in UIManager!");
        }

        CCPanelCards.SetActive(true);
    }

    /// <summary>
    /// Transforms the position of the card image in the Community Chest Card panel.
    /// </summary>
    /// <param name="newPosition">The new local position for the card image.</param>
    public void TransformCardImage(Vector2 newPosition)
    {
        if (cardImage == null)
        {
            Debug.LogWarning("cardImage is not assigned in UIManager!");
            return;
        }

        cardImage.rectTransform.anchoredPosition = newPosition;
        Debug.Log($"Card image position set to: {newPosition}");
    }

    public void publicConfirmBuy()
    {
        if (buttonAudioSource != null && confirmBuyClip != null)
        {
            buttonAudioSource.PlayOneShot(confirmBuyClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play ConfirmBuy SFX: AudioSource or confirmBuyClip is missing.");
        }
        if (_currentCard != null && currentPlayer != null)
        {
            currentPlayer.inventory.Spend(ResourceType.ShinyPennies, _currentCard.cost);
            ApplyCardEffect();
        }
        else
        {
            Debug.LogWarning("Cannot confirm buy: currentCard or currentPlayer is null!");
        }
        HideCommunityChestCardDetails();
        if (TurnManager.GetCurrentPlayerIndex() == TurnManager.totalPlayers - 1)
        {
            // Show Round Effect CA panel for Player 4
            RandomEffectRounds randomEffects = FindFirstObjectByType<RandomEffectRounds>();
            if (randomEffects != null)
            {
                randomEffects.ApplyRandomEffectWithPanel(TurnManager.GetCurrentRound());
            }
        }
        else
        {
            if (turnController != null) turnController.UpdateTurnUI();
        }
        if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
    }

    public void publicCancelBuy()
    {
        if (buttonAudioSource != null && cancelBuyClip != null)
        {
            buttonAudioSource.PlayOneShot(cancelBuyClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play CancelBuy SFX: AudioSource or cancelBuyClip is missing.");
        }
        HideCommunityChestCardDetails();
        if (TurnManager.GetCurrentPlayerIndex() == TurnManager.totalPlayers - 1)
        {
            // Show Round Effect CA panel for Player 4
            RandomEffectRounds randomEffects = FindFirstObjectByType<RandomEffectRounds>();
            if (randomEffects != null)
            {
                randomEffects.ApplyRandomEffectWithPanel(TurnManager.GetCurrentRound());
            }
        }
        else
        {
            if (turnController != null) turnController.UpdateTurnUI();
        }
        if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
    }

    private void ApplyCardEffect()
    {
        CommunityChest chest = FindFirstObjectByType<CommunityChest>();
        if (chest != null && _currentCard != null && currentPlayer != null)
        {
            _currentCard.effect.Invoke(currentPlayer, _otherPlayers);
        }
        else
        {
            Debug.LogWarning("Cannot apply card effect: CommunityChest, currentCard, or currentPlayer is null!");
        }
    }

    public void HideCommunityChestCardDetails()
    {
        if (CCPanelCards != null) CCPanelCards.SetActive(false);
        if (CCPanel != null) CCPanel.SetActive(false);
    }

    public void ShowLoanOffer(PlayerController player, Action onDecision = null)
    {
        if (LOPanel == null)
        {
            Debug.LogWarning("LOPanel is not assigned in UIManager!");
            return;
        }
        currentPlayer = player;
        if (player.hasLoan)
        {
            ShowPaymentPanel(player, onDecision);
            return;
        }
        if (loanTitleText != null) loanTitleText.text = "Lucky Loan Lender";
        else Debug.LogWarning("loanTitleText is not assigned in UIManager!");
        if (loanDescriptionText != null)
            loanDescriptionText.text = player != null ?
            $"{player.name}: Enter 1-5 Shiny Pennies to borrow (repay double next round)!" :
            "Enter 1-5 Shiny Pennies to borrow (repay double next round)!";
        else Debug.LogWarning("loanDescriptionText is not assigned in UIManager!");
        if (loanAmountInput != null)
        {
            loanAmountInput.text = "";
            acceptLoanButton.interactable = false;
        }
        if (acceptLoanButton != null)
        {
            acceptLoanButton.onClick.RemoveAllListeners();
            acceptLoanButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                OnConfirmLoan();
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HideLoanOffer();
            });
        }
        else Debug.LogWarning("acceptLoanButton is not assigned in UIManager!");
        if (cancelLoanButton != null)
        {
            cancelLoanButton.onClick.RemoveAllListeners();
            cancelLoanButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HideLoanOffer();
            });
        }
        else Debug.LogWarning("cancelLoanButton is not assigned in UIManager!");
        LOPanel.SetActive(true);
    }

    public void ShowPaymentPanel(PlayerController player, Action onDecision = null)
    {
        if (LOPanelPayment == null)
        {
            Debug.LogWarning("LOPanelPayment is not assigned in UIManager!");
            return;
        }
        currentPlayer = player;
        int debtAmount = player.loanAmount * 2;
        if (paymentTitleText != null) paymentTitleText.text = "Repay Your Loan";
        else Debug.LogWarning("paymentTitleText is not assigned in UIManager!");
        if (paymentDescriptionText != null)
            paymentDescriptionText.text = player != null ?
            $"{player.name}: You owe {debtAmount} Shiny Pennies. Enter amount to repay." :
            $"You owe {debtAmount} Shiny Pennies. Enter amount to repay.";
        else Debug.LogWarning("paymentDescriptionText is not assigned in UIManager!");
        if (paymentAmountInput != null)
        {
            paymentAmountInput.text = "";
            confirmPaymentButton.interactable = false;
        }
        if (confirmPaymentButton != null)
        {
            confirmPaymentButton.onClick.RemoveAllListeners();
            confirmPaymentButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                OnConfirmPay();
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HidePaymentPanel();
            });
        }
        else Debug.LogWarning("confirmPaymentButton is not assigned in UIManager!");
        if (cancelPaymentButton != null)
        {
            cancelPaymentButton.onClick.RemoveAllListeners();
            cancelPaymentButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                OnCancelPay();
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HidePaymentPanel();
            });
        }
        else Debug.LogWarning("cancelPaymentButton is not assigned in UIManager!");
        LOPanelPayment.SetActive(true);
    }

    public void OnConfirmLoan()
    {
        if (currentPlayer != null && loanAmountInput != null)
        {
            if (int.TryParse(loanAmountInput.text, out int amount) && amount >= 1 && amount <= 5)
            {
                LuckyLoanLender loanLender = FindFirstObjectByType<LuckyLoanLender>();
                if (loanLender != null)
                {
                    loanLender.OfferLoan(currentPlayer, amount);
                    if (!currentPlayer.hasLoan)
                    {
                        Debug.LogWarning($"{currentPlayer.name}'s hasLoan was not set to true in OfferLoan!");
                        currentPlayer.hasLoan = true;
                    }
                }
                else Debug.LogWarning("LuckyLoanLender not found in scene!");
            }
            else
            {
                Debug.LogWarning("Invalid loan amount entered!");
            }
        }
    }

    public void OnCancelPay()
    {
        if (currentPlayer != null)
        {
            LuckyLoanLender loanLender = FindFirstObjectByType<LuckyLoanLender>();
            if (loanLender != null)
            {
                loanLender.RepayLoan(currentPlayer, 0);
                if (!currentPlayer.isInDebt)
                {
                    Debug.LogWarning($"{currentPlayer.name}'s isInDebt was not set to true in RepayLoan!");
                    currentPlayer.isInDebt = true;
                }
            }
            else Debug.LogWarning("LuckyLoanLender not found in scene!");
        }
    }

    public void OnConfirmPay()
    {
        if (currentPlayer != null && paymentAmountInput != null)
        {
            if (int.TryParse(paymentAmountInput.text, out int amount))
            {
                LuckyLoanLender loanLender = FindFirstObjectByType<LuckyLoanLender>();
                if (loanLender != null)
                {
                    loanLender.RepayLoan(currentPlayer, amount);
                }
                else Debug.LogWarning("LuckyLoanLender not found in scene!");
            }
            else
            {
                Debug.LogWarning("Invalid payment amount entered!");
            }
        }
    }

    public void HideLoanOffer()
    {
        if (LOPanel != null) LOPanel.SetActive(false);
    }

    public void HidePaymentPanel()
    {
        if (LOPanelPayment != null) LOPanelPayment.SetActive(false);
    }

    public void ShowExchange(Action onDecision = null)
    {
        if (REPanel == null)
        {
            Debug.LogWarning("REPanel is not assigned in UIManager!");
            return;
        }
        string currentPlayerTag = "Player" + (TurnManager.GetCurrentPlayerIndex() + 1);
        GameObject playerObject = GameObject.FindGameObjectWithTag(currentPlayerTag);
        if (playerObject == null)
        {
            Debug.LogWarning($"Player GameObject with tag '{currentPlayerTag}' not found.");
            return;
        }
        mintingPlayer = playerObject.GetComponent<PlayerController>();
        if (mintingPlayer == null)
        {
            Debug.LogWarning("PlayerController component not found on player GameObject.");
            return;
        }
        string description = "Royal Fickle Mint\nExchange 1 Bronze, 3 Silver, 6 Gold for 1 Shiny Penny.";
        var inv = mintingPlayer.inventory;
        if (inv == null)
        {
            Debug.LogWarning("Player inventory not found!");
            return;
        }
        bool canAfford = inv.CanAfford(ResourceType.Bronze, 1) &&
        inv.CanAfford(ResourceType.Silver, 3) &&
        inv.CanAfford(ResourceType.Gold, 6);
        bool canExchangeWithShortage = true;
        if (inv.IsInShortage)
        {
            int totalBronze = inv.BronzeValue - 1;
            int totalSilver = inv.SilverValue - 3;
            int totalGold = inv.GoldValue - 6;
            canExchangeWithShortage = totalBronze <= 20 && totalSilver <= 20 && totalGold <= 20;
        }
        bool isWrongCurrencyActive = false;
        string devaluedResource = "";
        if (WrongCurrency.Instance != null && (WrongCurrency.Instance.IsResourceDevalued(ResourceType.Bronze) ||
        WrongCurrency.Instance.IsResourceDevalued(ResourceType.Silver) ||
        WrongCurrency.Instance.IsResourceDevalued(ResourceType.Gold)))
        {
            isWrongCurrencyActive = true;
            devaluedResource = WrongCurrency.Instance.GetDevaluedResource().ToString();
        }
        if (!canAfford || !canExchangeWithShortage)
        {
            description += "\nInvalid Resources: You lack the required resources or exceed shortage limits!";
        }
        if (exchangeTitleText != null)
        {
            exchangeTitleText.text = description;
        }
        else Debug.LogWarning("exchangeTitleText is not assigned in UIManager!");
        if (bronzeInput != null) bronzeInput.text = "1 Bronze";
        else Debug.LogWarning("bronzeInput is not assigned in UIManager!");
        if (silverInput != null) silverInput.text = "3 Silver";
        else Debug.LogWarning("silverInput is not assigned in UIManager!");
        if (goldInput != null) goldInput.text = "6 Gold";
        else Debug.LogWarning("goldInput is not assigned in UIManager!");
        if (confirmExchangeButton != null)
        {
            confirmExchangeButton.interactable = canAfford && canExchangeWithShortage && !isWrongCurrencyActive;
            confirmExchangeButton.onClick.RemoveAllListeners();
            confirmExchangeButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                ConfirmExchange();
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HideExchange();
            });
        }
        else Debug.LogWarning("confirmExchangeButton is not assigned in UIManager!");
        if (cancelExchangeButton != null)
        {
            cancelExchangeButton.onClick.RemoveAllListeners();
            cancelExchangeButton.onClick.AddListener(() =>
            {
                if (buttonAudioSource != null && buttonClickClip != null)
                {
                    buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
                }
                else
                {
                    Debug.LogWarning("Cannot play button click sound: AudioSource or AudioClip is missing.");
                }
                onDecision?.Invoke();
                if (Dice.Instance != null) Dice.Instance.OnActionConfirmed();
                HideExchange();
                if (turnController != null)
                {
                    turnController.UpdateTurnUI();
                }
                else
                {
                    Debug.LogWarning("TurnUIController not found to update next player's turn!");
                }
            });
        }
        else Debug.LogWarning("cancelExchangeButton is not assigned in UIManager!");
        REPanel.SetActive(true);
    }

    public void ConfirmExchange()
    {
        if (mintingPlayer == null)
        {
            Debug.LogWarning("No minting player assigned for exchange!");
            HideExchange();
            return;
        }
        var inv = mintingPlayer.inventory;
        if (inv == null)
        {
            Debug.LogWarning("Player inventory not found!");
            HideExchange();
            return;
        }
        int bronzeToSpend = 1;
        int silverToSpend = 3;
        int goldToSpend = 6;
        int shinyPenniesToGain = 1;
        if (!inv.CanAfford(ResourceType.Bronze, bronzeToSpend) ||
        !inv.CanAfford(ResourceType.Silver, silverToSpend) ||
        !inv.CanAfford(ResourceType.Gold, goldToSpend))
        {
            Debug.LogWarning($"{mintingPlayer.name} cannot afford to exchange 1 Bronze, 3 Silver, and 6 Gold!");
            return;
        }
        if (inv.IsInShortage)
        {
            int totalBronze = inv.BronzeValue - bronzeToSpend;
            int totalSilver = inv.SilverValue - silverToSpend;
            int totalGold = inv.GoldValue - goldToSpend;
            if (totalBronze > 20 || totalSilver > 20 || totalGold > 20)
            {
                Debug.LogWarning($"{mintingPlayer.name} cannot exchange due to shortage restrictions (max 20 per metal resource).");
                return;
            }
        }
        if (WrongCurrency.Instance != null && (
        WrongCurrency.Instance.IsResourceDevalued(ResourceType.Bronze) ||
        WrongCurrency.Instance.IsResourceDevalued(ResourceType.Silver) ||
        WrongCurrency.Instance.IsResourceDevalued(ResourceType.Gold)))
        {
            Debug.LogWarning($"{mintingPlayer.name} cannot exchange due to Wrong Currency effect!");
            return;
        }
        inv.Spend(ResourceType.Bronze, bronzeToSpend);
        inv.Spend(ResourceType.Silver, silverToSpend);
        inv.Spend(ResourceType.Gold, goldToSpend);
        inv.Add(ResourceType.ShinyPennies, shinyPenniesToGain);
        Debug.Log($"{mintingPlayer.name} exchanged 1 Bronze, 3 Silver, 6 Gold for 1 Shiny Penny.");
        if (turnController != null)
        {
            turnController.UpdateTurnUI();
        }
        else
        {
            Debug.LogWarning("TurnUIController not found to update next player's turn!");
        }
    }

    public void HideExchange()
    {
        if (REPanel != null) REPanel.SetActive(false);
    }
}
