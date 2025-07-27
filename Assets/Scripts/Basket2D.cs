using UnityEngine;
using System.Collections.Generic;

public class Basket2D : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform ingredientsContainer;
    public GameObject ingredientPrefab;
    
    [Header("Ingredients Display")]
    public int ingredientsToDraw = 5;
    public float ingredientSpacing = 1.5f;
    
    private List<GameObject> drawnIngredients = new List<GameObject>();
    
    void Start()
    {
        DrawIngredients();
    }
    
    void DrawIngredients()
    {
        for (int i = 0; i < ingredientsToDraw; i++)
        {
            Ingredient ingredient = deckManager.DrawIngredient();
            if (ingredient == null) break;
            CreateIngredient(ingredient, i);
        }
    }
    
    void CreateIngredient(Ingredient ingredient, int index)
    {
        GameObject ingredientObj = Instantiate(ingredientPrefab);
        ingredientObj.name = $"Ingredient_{ingredient.ingredientName}";
        
        float totalWidth = (ingredientsToDraw - 1) * ingredientSpacing;
        float startX = -totalWidth / 2f;
        Vector3 ingredientPosition = ingredientsContainer.position + new Vector3(startX + index * ingredientSpacing, 0, 0);
        ingredientObj.transform.position = ingredientPosition;
        
        IngredientPrefab ingredientScript = ingredientObj.GetComponent<IngredientPrefab>();
        if (ingredientScript != null) ingredientScript.SetIngredient(ingredient);
        drawnIngredients.Add(ingredientObj);
    }
}
