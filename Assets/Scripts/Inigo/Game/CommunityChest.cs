using System;
using System.Collections.Generic;
using UnityEngine;

public class CommunityChest : MonoBehaviour
{
    // ---------- Nested types ----------
    [Serializable]
    public class CommunityCard
    {
        public string cardName;
        public string description;
        public int cost; // shiny-penny cost to draw
        public Sprite cardImage; // Sprite for the card's image
        public Action<PlayerController, List<PlayerController>> effect;
    }

    // ---------- Fields ----------
    public readonly List<CommunityCard> _deck = new(); // run-time deck, made public for UIManager access

    // ---------- Inspector Fields ----------
    [SerializeField] private Sprite stealGoldSprite; // Assign in Inspector
    [SerializeField] private Sprite stealSilverSprite;
    [SerializeField] private Sprite stealBronzeSprite;
    [SerializeField] private Sprite stealPenniesSprite;
    [SerializeField] private Sprite gainPenniesSprite;

    // ---------- Unity ----------
    private void Awake() => InitialiseDeck();

    // ---------- Private helpers ----------
    private void InitialiseDeck()
    {
        _deck.Clear();

        _deck.AddRange(new[]
        {
            NewCard("Steal Gold", "Random player decrease 2 gold.", 8, stealGoldSprite,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(2, target.inventory.GoldValue);
                    target.inventory.Spend(ResourceType.Gold, lost);
                }),

            NewCard("Steal Silver", "Random player and decrease 5 silver.", 5, stealSilverSprite,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(5, target.inventory.SilverValue);
                    target.inventory.Spend(ResourceType.Silver, lost);
                }),

            NewCard("Steal Bronze", "Random player and decrease 10 bronze.", 1, stealBronzeSprite,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(10, target.inventory.BronzeValue);
                    target.inventory.Spend(ResourceType.Bronze, lost);
                }),

            NewCard("Steal Shiny Pennies", "Random player and decrease 5 shiny pennies.", 12, stealPenniesSprite,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int stolen = Mathf.Min(5, target.inventory.ShinyPenniesValue);
                    target.inventory.Spend(ResourceType.ShinyPennies, stolen);
                    self.inventory.Add(ResourceType.ShinyPennies, stolen);
                }),

            NewCard("Gain Extra Pennies", "Gain 10 shiny pennies.", 5, gainPenniesSprite,
                (self, _) =>
                {
                    self.inventory.Add(ResourceType.ShinyPennies, 10);
                }),
        });
    }

    private static CommunityCard NewCard(string name, string desc, int cost, Sprite sprite,
        Action<PlayerController, List<PlayerController>> effect) =>
        new() { cardName = name, description = desc, cost = cost, cardImage = sprite, effect = effect };

    private static PlayerController ChooseTarget(IReadOnlyList<PlayerController> players) =>
        players[UnityEngine.Random.Range(0, players.Count)];
}
