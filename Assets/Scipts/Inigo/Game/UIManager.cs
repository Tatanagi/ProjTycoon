using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject CCPanel;
    public GameObject LOPanel;
    public TextMeshProUGUI cardTitleText;
    public TextMeshProUGUI cardDescriptionText;
    public TextMeshProUGUI loanTitleText;
    public TextMeshProUGUI loanDescriptionText;
    public Button drawCardButton;
    public Button acceptLoanButton;
    public Button cancelLoanButton;

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

        // Hide UI at start
        CCPanel.SetActive(false);
        LOPanel.SetActive(false);
    }

    // --- COMMUNITY CHEST ---

    public void ShowCommunityChestCard(string title, string description)
    {
        CCPanel.SetActive(true);
        cardTitleText.text = "Community Chest";
        cardDescriptionText.text = $"Cost: X Shiny Pennies\nClick to draw a random effect!";
    }

    public void HideCommunityChestCard()
    {
        CCPanel.SetActive(false);
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

    // --- LUCKY LOAN LENDER ---

    public void ShowLoanOffer(PlayerController player)
    {
        currentPlayer = player;
        loanTitleText.text = "Lucky Loan Lender";
        loanDescriptionText.text = $"Would you like to take a loan worth 10% of your current shiny pennies?";

        LOPanel.SetActive(true);
    }

    public void OnConfirmLoan()
    {
        GameManager.Instance.loanLender.OfferLoan(currentPlayer);
        LOPanel.SetActive(false);
    }

    public void HideLoanOffer()
    {
        LOPanel.SetActive(false);
    }
}
