using UnityEngine;
using System.Collections.Generic;

public class Basket2D : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform ingredientsContainer; // GameObject vide où centrer les ingrédients
    
    [Header("Ingredients Display")]
    public int ingredientsToDraw = 5;
    public float ingredientSpacing = 1.5f;
    
    [Header("Ingredient Size")]
    public Vector2 ingredientSize = new Vector2(1f, 1f);
    
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
        // Créer un nouvel objet pour l'ingrédient
        GameObject ingredientObj = new GameObject($"Ingredient_{ingredient.ingredientName}");
        
        // Calculer la position centrée
        float totalWidth = (ingredientsToDraw - 1) * ingredientSpacing;
        float startX = -totalWidth / 2f;
        Vector3 ingredientPosition = ingredientsContainer.position + new Vector3(startX + index * ingredientSpacing, 0, 0);
        ingredientObj.transform.position = ingredientPosition;
        
        // Ajouter le sprite renderer
        SpriteRenderer spriteRenderer = ingredientObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = ingredient.icon;
        spriteRenderer.sortingOrder = 2;
        
        // Ajuster la taille du sprite
        ingredientObj.transform.localScale = new Vector3(ingredientSize.x, ingredientSize.y, 1f);
        
        // Ajouter un collider pour les interactions (ajusté à la taille)
        BoxCollider2D collider = ingredientObj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one; // Le collider sera automatiquement ajusté par le scale
        
        drawnIngredients.Add(ingredientObj);
    }
}
