using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draw‑a‑card mechanic that lets a player pay a cost to trigger an effect.
/// The deck is created at runtime in <see cref="Awake"/>.
/// </summary>
public class CommunityChest : MonoBehaviour
{
    // ---------- Nested types ----------

    [Serializable]
    public class CommunityCard
    {
        public string cardName;
        public string description;
        public int cost;   // shiny‑penny cost to draw
        public Action<PlayerController, List<PlayerController>> effect;
    }

    // ---------- Fields ----------

    private readonly List<CommunityCard> _deck = new();   // run‑time deck

    // ---------- Unity ----------

    private void Awake() => InitialiseDeck();

    // ---------- Public API ----------

    /// <summary>Draws (and immediately resolves) a random card, if the player can afford the cost.</summary>
    public void DrawCard(PlayerController player, IReadOnlyList<PlayerController> allPlayers)
    {
        if (_deck.Count == 0) return;

        CommunityCard card = _deck[UnityEngine.Random.Range(0, _deck.Count)];

        if (player.shinyPennies < card.cost)
        {
            Debug.Log($"{player.name} doesn't have enough shiny pennies.");
            return;
        }

        player.shinyPennies -= card.cost;
        card.effect.Invoke(player, new List<PlayerController>(allPlayers).FindAll(p => p != player));

        Debug.Log($"{player.name} drew Community Card: {card.cardName} – {card.description}");
    }

    // ---------- Private helpers ----------

    private void InitialiseDeck()
    {
        _deck.Clear();
        _deck.AddRange(new[]
        {
            NewCard("Steal Gold",   "Pick a player and decrease 2 gold.",           8,
                (self, others) => ChooseTarget(others).gold   = Mathf.Max(0, ChooseTarget(others).gold   - 2)),

            NewCard("Steal Silver", "Pick a player and decrease 5 silver.",         5,
                (self, others) => ChooseTarget(others).silver = Mathf.Max(0, ChooseTarget(others).silver - 5)),

            NewCard("Steal Bronze", "Pick a player and decrease 10 bronze.",        1,
                (self, others) => ChooseTarget(others).bronze = Mathf.Max(0, ChooseTarget(others).bronze - 10)),

            NewCard("Steal Shiny Pennies", "Pick a player and decrease 5 shiny pennies.", 12,
                (self, others) =>
                {
                    PlayerController target = ChooseTarget(others);
                    int stolen = Mathf.Min(5, target.shinyPennies);
                    target.shinyPennies -= stolen;
                    self.shinyPennies   += stolen;
                }),

            NewCard("Gain Extra Pennies", "Gain 10 shiny pennies.", 5,
                (self, _) => self.shinyPennies += 10),

            NewCard("Gain Turnips", "All players gain 25 turnips this round only.", 4,
                (self, others) =>
                {
                    foreach (var p in others) p.roundTurnips += 25;
                    self.roundTurnips += 25;
                })
        });
    }

    private static CommunityCard NewCard(string name, string desc, int cost,
        Action<PlayerController, List<PlayerController>> effect) =>
        new() { cardName = name, description = desc, cost = cost, effect = effect };

    private static PlayerController ChooseTarget(IReadOnlyList<PlayerController> players) =>
        players[UnityEngine.Random.Range(0, players.Count)];
}
