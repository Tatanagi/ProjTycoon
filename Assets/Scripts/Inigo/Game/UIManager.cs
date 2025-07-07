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
        int b = int.Parse(bronzeInput.text);
        int s = int.Parse(silverInput.text);
        int g = int.Parse(goldInput.text);

        var inv = mintingPlayer.inventory;
        var decree = RoyalDecreeManager.Instance;

        int bronzeSpent = Mathf.Min(b, inv.Bronze);
        int silverSpent = Mathf.Min(s, inv.Silver);
        int goldSpent = Mathf.Min(g, inv.Gold);

        int totalPennies =
              decree.GetValueExchange(ResourceType.Bronze, bronzeSpent)
            + decree.GetValueExchange(ResourceType.Silver, silverSpent)
            + decree.GetValueExchange(ResourceType.Gold, goldSpent);

        inv.Spend(ResourceType.Bronze, bronzeSpent);
        inv.Spend(ResourceType.Silver, silverSpent);
        inv.Spend(ResourceType.Gold, goldSpent);
        inv.Add(ResourceType.ShinyPennies, totalPennies);

        Debug.Log($"{mintingPlayer.name} exchanged for {totalPennies} shiny pennies.");
        HideExchange();
    }


    public void HideExchange()
    {
        REPanel.SetActive(false);
        if (turnController != null)
            turnController.UpdateTurnUI();
    }
}
