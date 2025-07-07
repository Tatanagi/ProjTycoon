using UnityEngine;

public class LuckyLoanLender : MonoBehaviour
{
    public void OfferLoan(PlayerController player)
    {
        if (!player.hasLoan)
        {
            int currentPennies = player.inventory.ShinyPennies;
            int loanAmount = Mathf.CeilToInt(currentPennies * 0.1f);

            player.inventory.Add(ResourceType.ShinyPennies, loanAmount);
            player.hasLoan = true;

            Debug.Log($"{player.name} took a loan of {loanAmount} shiny pennies.");
        }
        else
        {
            Debug.Log($"{player.name} already has a loan.");
        }
    }
}
