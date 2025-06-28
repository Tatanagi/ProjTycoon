using UnityEngine;

public class LuckyLoanLender : MonoBehaviour
{
    public void OfferLoan(PlayerController player)
    {
        if (!player.hasLoan)
        {
            int loanAmount = Mathf.CeilToInt(player.shinyPennies * 0.1f);
            player.shinyPennies += loanAmount;
            player.hasLoan = true;
            Debug.Log($"{player.name} took a loan of {loanAmount} shiny pennies.");
        }
        else
        {
            Debug.Log($"{player.name} already has a loan.");
        }
    }
}
