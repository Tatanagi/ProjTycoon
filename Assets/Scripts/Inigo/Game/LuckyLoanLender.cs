using UnityEngine;

public class LuckyLoanLender : MonoBehaviour
{
    public void OfferLoan(PlayerController player, int loanAmount)
    {
        if (!player.hasLoan && loanAmount >= 1 && loanAmount <= 5)
        {
            player.inventory.Add(ResourceType.ShinyPennies, loanAmount);
            player.hasLoan = true;
            player.loanAmount = loanAmount;
            player.isInDebt = true; // Set debt status immediately upon taking loan
            Debug.Log($"{player.name} took a loan of {loanAmount} shiny pennies. Must repay {loanAmount * 2} next round or remain in debt for one round.");
        }
        else
        {
            Debug.LogWarning($"{player.name} cannot take a loan: {(player.hasLoan ? "Already has a loan" : "Invalid loan amount")}.");
        }
    }

    public void RepayLoan(PlayerController player, int paymentAmount)
    {
        if (!player.hasLoan) return;

        int debtAmount = player.loanAmount * 2;
        if (paymentAmount >= debtAmount && player.inventory.CanAfford(ResourceType.ShinyPennies, paymentAmount))
        {
            player.inventory.Spend(ResourceType.ShinyPennies, paymentAmount);
            player.hasLoan = false;
            player.loanAmount = 0;
            player.isInDebt = false;
            Debug.Log($"{player.name} repaid loan of {paymentAmount} shiny pennies.");
        }
        else
        {
            Debug.LogWarning($"{player.name} failed to repay loan: Insufficient funds or invalid payment amount. Debt penalty applies for one round.");
            player.isInDebt = true; // Debt persists for the round
            player.hasLoan = false;
            player.loanAmount = 0;
        }
    }
}