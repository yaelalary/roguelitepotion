using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IngredientUIManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject ingredientUIPrefab;
    public Transform ingredientsSlotsParent;
    public int numberToDraw = 5;
    public float spacing = 100f;
    public List<Ingredient> drawnIngredients = new List<Ingredient>();

    void Start()
    {
        Debug.Log("IngredientUIManager started");
        DisplayIngredients();
    }

    void DisplayIngredients()
    {
        drawnIngredients.Clear();
        for (int i = 0; i < numberToDraw; i++)
        {
            Ingredient ingredient = deckManager.DrawIngredient();
            if (ingredient == null) break;
            drawnIngredients.Add(ingredient);
        }

        float startX = -((drawnIngredients.Count - 1) * spacing) / 2f;
        for (int i = 0; i < drawnIngredients.Count; i++)
        {
            GameObject uiObj = Instantiate(ingredientUIPrefab, ingredientsSlotsParent);
            uiObj.name = drawnIngredients[i].ingredientName;
            uiObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * spacing, 0);

            IngredientUI ui = uiObj.GetComponent<IngredientUI>();
            ui.SetIngredient(drawnIngredients[i]);
        }
    }
}