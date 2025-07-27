using UnityEngine;
using System.Collections.Generic;

public class Basket2D : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform ingredientsContainer; // GameObject vide où centrer les ingrédients
    public GameObject ingredientPrefab; // Prefab d'ingrédient à instancier
    
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
            
            CreateIngredientSprite(ingredient, i);
        }
        
        Debug.Log($"5 ingrédients piochés et affichés depuis le panier");
    }
    
    void CreateIngredientSprite(Ingredient ingredient, int index)
    {
        // Instancier le prefab d'ingrédient
        GameObject ingredientObj = Instantiate(ingredientPrefab);
        ingredientObj.name = $"Ingredient_{ingredient.ingredientName}";
        
        // Calculer la position centrée
        float totalWidth = (ingredientsToDraw - 1) * ingredientSpacing;
        float startX = -totalWidth / 2f;
        Vector3 ingredientPosition = ingredientsContainer.position + new Vector3(startX + index * ingredientSpacing, 0, 0);
        ingredientObj.transform.position = ingredientPosition;
        
        // Configurer l'ingrédient
        IngredientPrefab ingredientScript = ingredientObj.GetComponent<IngredientPrefab>();
        if (ingredientScript != null)
        {
            ingredientScript.SetIngredient(ingredient);
        }
        
        drawnIngredients.Add(ingredientObj);
    }
}
