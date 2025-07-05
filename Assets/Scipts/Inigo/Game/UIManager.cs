using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject CCPanel;
    public GameObject LOPanel;
    public GameObject REPanel;

    [Header("Community Chest")]
    public TextMeshProUGUI cardTitleText;
    public TextMeshProUGUI cardDescriptionText;
    public Button drawCardButton;

    [Header("Lucky Loan Lender")]
    public TextMeshProUGUI loanTitleText;
    public TextMeshProUGUI loanDescriptionText;
    public Button acceptLoanButton;
    public Button cancelLoanButton;

    [Header("Royal Fickle Mint")]
    public TextMeshProUGUI exchangeTitleText;
    public TextMeshProUGUI bronzeInput;
    public TextMeshProUGUI silverInput;
    public TextMeshProUGUI goldInput;
    public Button confirmExchangeButton;
    public Button cancelExchangeButton;

    [Header("Turn UI Controller")]
    public TurnUIController turnController;

    private PlayerController currentPlayer;

    private PlayerController mintingPlayer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (turnController == null)
            turnController = FindFirstObjectByType<TurnUIController>();

        CCPanel.SetActive(false);
        LOPanel.SetActive(false);
        REPanel.SetActive(false);
    }

    // --- COMMUNITY CHEST ---

    public void ShowCommunityChestCard(PlayerController player)
    {
        cardTitleText.text = "Community Chest";
        cardDescriptionText.text = $"Cost: X Shiny Pennies\nClick to draw a random effect!";

        CCPanel.SetActive(true);
    }

    public void ShowDrawCard(PlayerController player)
    {
        currentPlayer = player;

        drawCardButton.onClick.RemoveAllListeners();
        drawCardButton.onClick.AddListener(() =>
        {
            CommunityChest chest = FindFirstObjectByType<CommunityChest>();
            chest.DrawCard(currentPlayer, GameManager.Instance.GetAllPlayers());
            HideCommunityChestCard();
        });

        CCPanel.SetActive(true);
    }

    public void HideCommunityChestCard()
    {
        CCPanel.SetActive(false);

        if (turnController != null) 
            turnController.UpdateTurnUI();
    }

        // --- LUCKY LOAN LENDER ---

        public void ShowLoanOffer(PlayerController player)
    {
        currentPlayer = player;

        loanTitleText.text = "Lucky Loan Lender";
        loanDescriptionText.text = $"Would you like to take a loan worth 10% of your current shiny pennies?";

        acceptLoanButton.onClick.RemoveAllListeners();
        cancelLoanButton.onClick.RemoveAllListeners();

        acceptLoanButton.onClick.AddListener(OnConfirmLoan);
        cancelLoanButton.onClick.AddListener(HideLoanOffer);

        LOPanel.SetActive(true);
    }

    public void OnConfirmLoan()
    {
        GameManager.Instance.loanLender.OfferLoan(currentPlayer);
        HideLoanOffer();
    }

    public void HideLoanOffer() 
    { 
        LOPanel.SetActive(false);

        if (turnController != null)
            turnController.UpdateTurnUI();
    }

        // --- ROYAL FICKLE MINT ---
    public void ShowExchange(PlayerController player)
    {
        mintingPlayer = player;

        exchangeTitleText.text = "Royal Fickle Mint";
        bronzeInput.text = "0";
        silverInput.text = "0";
        goldInput.text = "0";

        confirmExchangeButton.onClick.RemoveAllListeners();
        cancelExchangeButton.onClick.RemoveAllListeners();

        confirmExchangeButton.onClick.AddListener(ConfirmExchange);
        cancelExchangeButton.onClick.AddListener(HideExchange);

        REPanel.SetActive(true);
    }

    private void ConfirmExchange()
    {
        int bronze = int.Parse(bronzeInput.text);
        int silver = int.Parse(silverInput.text);
        int gold = int.Parse(goldInput.text);

        RoyalDecreeManager decree = RoyalDecreeManager.Instance;

        int bronzeValue = decree.GetValueExchange(ResourceType.Bronze, Mathf.Min(bronze, mintingPlayer.bronze));
        int silverValue = decree.GetValueExchange(ResourceType.Silver, Mathf.Min(silver, mintingPlayer.silver));
        int goldValue = decree.GetValueExchange(ResourceType.Silver, Mathf.Min(gold, mintingPlayer.gold));

        int total = bronzeValue + silverValue + goldValue;

        mintingPlayer.bronze -= Mathf.Min(bronze, mintingPlayer.bronze);
        mintingPlayer.silver -= Mathf.Min(silver, mintingPlayer.silver);
        mintingPlayer.gold -= Mathf.Min(gold, mintingPlayer.gold);

        mintingPlayer.shinyPennies += total;

        Debug.Log($"{mintingPlayer.name} exchanged resources for {total} shiny pennies.");

        HideExchange();
    }

    public void HideExchange()
    {
        REPanel.SetActive(false);
        if (turnController != null)
            turnController.UpdateTurnUI();
    }
}
