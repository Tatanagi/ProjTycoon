using UnityEngine;

/// <summary>
/// Trigger on the Start tile that grants bonus only after Round 1.
/// </summary>
public class StartTileBonus : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2") ||
            other.CompareTag("Player3") || other.CompareTag("Player4"))
        {
            if (GameManager.Instance.round < 2) return;

            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                GameManager.Instance.resourceMarket.GiveStartTileBonus(player);
            }
        }
    }
}
