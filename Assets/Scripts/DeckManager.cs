using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Ingredient[] allIngredients;
    public int copiesPerIngredient = 4;
    public List<Ingredient> deck = new List<Ingredient>();

    void Awake()
    {
        GenerateDefaultDeck();
    }

    void GenerateDefaultDeck()
    {
        deck.Clear();
        foreach (var ingredient in allIngredients)
        {
            for (int i = 0; i < copiesPerIngredient; i++) deck.Add(ingredient);
        }
        Debug.Log("Deck generated with " + deck.Count + " cards.");
    }

    public Ingredient DrawIngredient()
    {
        if (deck.Count == 0) return null;
        int index = Random.Range(0, deck.Count);
        Ingredient drawn = deck[index];
        deck.RemoveAt(index);
        return drawn;
    }
}
