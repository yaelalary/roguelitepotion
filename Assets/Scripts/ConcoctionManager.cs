using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    
    [Header("Game References")]
    public Basket2D basket;
    public PotionRecipeBook recipeBook;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    
    void Start()
    {
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(false);
            concoctButton.onClick.AddListener(ConcoctPotion);
        }
        
        // Ensure we have a recipe book
        EnsureRecipeBookExists();
    }
    
    void EnsureRecipeBookExists()
    {
        if (recipeBook == null)
        {
            Debug.LogWarning("Recipe book not assigned! Trying to find or create one...");
            
            // Try to find an existing recipe book in the project
            var foundRecipeBooks = Resources.FindObjectsOfTypeAll<PotionRecipeBook>();
            if (foundRecipeBooks.Length > 0)
            {
                recipeBook = foundRecipeBooks[0];
                Debug.Log($"Found recipe book with {recipeBook.recipes.Count} recipes");
            }
            else
            {
                Debug.LogWarning("No recipe book found. Creating a basic one...");
                CreateBasicRecipeBook();
            }
        }
    }
    
    void CreateBasicRecipeBook()
    {
        recipeBook = ScriptableObject.CreateInstance<PotionRecipeBook>();
        
        // Add a simple test recipe
        var testRecipe = new PotionRecipe();
        testRecipe.potionName = "AUTO - Simple Magic Potion";
        testRecipe.description = "Auto-created test recipe";
        testRecipe.baseScore = 50;
        testRecipe.minIngredients = 1;
        testRecipe.maxIngredients = 4;
        testRecipe.categoryRequirements = new List<CategoryRequirement>
        {
            new CategoryRequirement
            {
                type = RequirementType.Family,
                targetFamily = IngredientFamily.Magic,
                minCount = 1,
                maxCount = 4
            }
        };
        
        recipeBook.recipes.Add(testRecipe);
        
        Debug.Log("Created basic recipe book with 1 test recipe");
    }
    
    public void AddSelectedIngredient(IngredientPrefab ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateButtonVisibility();
        }
    }
    
    public void RemoveSelectedIngredient(IngredientPrefab ingredient)
    {
        selectedIngredients.Remove(ingredient);
        UpdateButtonVisibility();
    }
    
    void UpdateButtonVisibility()
    {
        if (concoctButton != null) concoctButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }

    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0) return;
        
        // Get the ingredients from selected prefabs
        List<Ingredient> ingredients = new List<Ingredient>();
        Debug.Log("=== DEBUGGING CONCOCTION ===");
        Debug.Log($"Nombre d'ingrédients sélectionnés: {selectedIngredients.Count}");
        
        foreach (var ingredientPrefab in selectedIngredients)
        {
            var ingredient = ingredientPrefab.GetIngredient();
            ingredients.Add(ingredient);
            Debug.Log($"Ingrédient: {ingredient.IngredientName} | Family: {ingredient.family} | SubFamily: {ingredient.subFamily}");
        }
        
        // Check for magic ingredients manually
        int magicCount = 0;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.family == IngredientFamily.Magic)
            {
                magicCount++;
            }
        }
        Debug.Log($"Nombre d'ingrédients magiques détectés: {magicCount}");
        
        // Find matching recipe
        PotionRecipe matchedRecipe = null;
        if (recipeBook != null)
        {
            Debug.Log($"Recipe book contient {recipeBook.recipes.Count} recettes");
            matchedRecipe = recipeBook.FindBestRecipe(ingredients);
        }
        else
        {
            Debug.LogError("Recipe book est null!");
        }
        
        if (matchedRecipe != null)
        {
            // Successfully created a potion!
            int score = matchedRecipe.CalculateScore(ingredients);
            
            Debug.Log($"=== POTION CREATED ===");
            Debug.Log($"Potion: {matchedRecipe.potionName}");
            Debug.Log($"Description: {matchedRecipe.description}");
            Debug.Log($"Score: {score} points");
            Debug.Log($"Ingredients used:");
            
            foreach (var ingredientPrefab in selectedIngredients)
            {
                var ingredient = ingredientPrefab.GetIngredient();
                Debug.Log($"- {ingredient.IngredientName} ({ingredient.family}, {ingredient.subFamily})");
            }
            
            // TODO: Add potion to inventory, update score, show success animation
        }
        else
        {
            // Failed to create potion
            Debug.Log("=== POTION FAILED ===");
            Debug.Log("These ingredients don't make a valid potion!");
            Debug.Log("Requirements not met:");
            Debug.Log("- Must have at least 1 Magic ingredient");
            Debug.Log("- Must match a known recipe pattern");
            
            // TODO: Show failure feedback, maybe return ingredients or penalty
            return; // Don't remove ingredients if potion failed
        }
        
        // Remove used ingredients and hide button
        RemoveUsedIngredients();
    }
    
    void RemoveUsedIngredients()
    {
        if (basket != null) basket.ReplaceIngredients(selectedIngredients);
        else
        {
            foreach (var ingredientPrefab in selectedIngredients) Destroy(ingredientPrefab.gameObject);
        }
        selectedIngredients.Clear();
        UpdateButtonVisibility();
    }
    
    public List<IngredientPrefab> GetSelectedIngredients()
    {
        return new List<IngredientPrefab>(selectedIngredients);
    }
}
