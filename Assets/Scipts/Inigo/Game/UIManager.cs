using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject CCPanel;
    public GameObject LOPanel;

    [Header("Community Chest")]
    public TextMeshProUGUI cardTitleText;
    public TextMeshProUGUI cardDescriptionText;
    public Button drawCardButton;

    [Header("Lucky Loan Lender")]
    public TextMeshProUGUI loanTitleText;
    public TextMeshProUGUI loanDescriptionText;
    public Button acceptLoanButton;
    public Button cancelLoanButton;

    [Header("Turn UI Controller")]
    public TurnUIController turnController;

    private PlayerController currentPlayer;

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
}
