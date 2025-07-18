using UnityEngine;

public class StartTileBonus : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2") ||
            other.CompareTag("Player3") || other.CompareTag("Player4"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                GameManager.Instance.GiveStartTileBonus(player);
                Debug.Log($"{player.name} landed on Start and received +5 gold, silver, and bronze!");
            }
        }
    }
}
