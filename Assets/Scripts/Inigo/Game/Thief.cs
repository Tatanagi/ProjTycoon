using UnityEngine;

public static class Thief
{
    public static void Execute(PlayerController player)
    {
        var inv = player.inventory;

        int stolenBronze = Mathf.FloorToInt(inv.Bronze * 0.20f);
        int stolenSilver = Mathf.FloorToInt(inv.Silver * 0.10f);
        int stolenGold = Mathf.FloorToInt(inv.Gold * 0.05f);

        inv.Bronze = Mathf.Max(0, inv.Bronze - stolenBronze);
        inv.Silver = Mathf.Max(0, inv.Silver - stolenSilver);
        inv.Gold = Mathf.Max(0, inv.Gold - stolenGold);

        Debug.Log($"{player.name} was robbed! Lost {stolenBronze} 🟤, {stolenSilver} ⚪, {stolenGold} 🟡");
        UIManager.Instance.ShowCellAction
        (
            "Thief",
            $"{player.name} was robbed!\nLost {stolenBronze} Bronze, {stolenSilver} Silver, {stolenGold} Gold.",
            player
        );
    }
}
