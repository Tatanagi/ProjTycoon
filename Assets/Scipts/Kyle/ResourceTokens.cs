using UnityEngine;

public class ResourceTokens : MonoBehaviour
{
    public int goldAmount = 5;
    public int silverAmount = 5;
    public int bronzeAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a Player
        if (other.CompareTag("Player1") || other.CompareTag("Player2") ||
            other.CompareTag("Player3") || other.CompareTag("Player4"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.gold += goldAmount;
                player.silver += silverAmount;
                player.bronze += bronzeAmount;

                Debug.Log($"{other.tag} gained {goldAmount} Gold, {silverAmount} Silver, {bronzeAmount} Bronze on Start tile.");
            }
        }
    }
}
