using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    
    [Header("Game References")]
    public Basket2D basket;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    
    void Start()
    {
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(false);
            concoctButton.onClick.AddListener(ConcoctPotion);
        }
        
        CreateAllRecipes();
    }
    
    void CreateAllRecipes()
    {
        recipes.Clear();
        
        // === 1 INGREDIENT RECIPES ===
        CreateRecipe("Magic Plant Potion", "A basic potion made from one magic plant", 1, 1, 30,
                    MagicPlantRequirement(1, 1));
        
        CreateRecipe("Magic Animal Potion", "A basic potion made from one magic animal", 1, 1, 30,
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Mineral Potion", "A powerful potion made from one magic mineral", 2, 1, 60,
                    MagicMineralRequirement(1, 1));
        
        // === 2 INGREDIENT RECIPES - SAME TYPE ===
        CreateRecipe("Double Magic Plant Potion", "Enhanced plant magic", 2, 1, 45,
                    MagicPlantRequirement(2, 2));
        
        CreateRecipe("Double Magic Animal Potion", "Enhanced animal magic", 2, 1, 45,
                    MagicAnimalRequirement(2, 2));
        
        CreateRecipe("Double Magic Mineral Potion", "Very powerful mineral magic", 2, 1, 45,
                    MagicMineralRequirement(2, 2));
        
        // === 2 INGREDIENT RECIPES - MIXED MAGIC ===
        CreateRecipe("Magic Plant & Animal Potion", "Mixed essence of plant and animal", 2, 1, 30,
                    MagicPlantRequirement(1, 1),
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Plant & Mineral Potion", "Mixed essence of plant and mineral", 2, 1, 30,
                    MagicPlantRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        CreateRecipe("Magic Animal & Mineral Potion", "Mixed essence of animal and mineral", 2, 1, 30,
                    MagicAnimalRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        // === 2 INGREDIENT RECIPES - MAGIC + NATURAL ===
        CreateRecipe("Hybrid Plant Potion", "Magic and natural plant fusion", 2, 1, 30,
                    MagicPlantRequirement(1, 1),
                    NaturalPlantRequirement(1, 1));
        
        CreateRecipe("Hybrid Animal Potion", "Magic and natural animal fusion", 2, 1, 30,
                    MagicAnimalRequirement(1, 1),
                    NaturalAnimalRequirement(1, 1));
        
        CreateRecipe("Hybrid Mineral Potion", "Magic and natural mineral fusion", 2, 1, 30,
                    MagicMineralRequirement(1, 1),
                    NaturalMineralRequirement(1, 1));

        Debug.Log($"Created {recipes.Count} recipes!");
    }
    
    void CreateRecipe(string name, string description, int level, int subLevel, int duration, params CategoryRequirement[] requirements)
    {
        recipes.Add(new PotionRecipe
        {
            potionName = name,
            description = description,
            level = level,
            subLevel = subLevel,
            duration = duration,
            categoryRequirements = new List<CategoryRequirement>(requirements)
        });
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
        Debug.Log($"Number of selected ingredients: {selectedIngredients.Count}");
        
        foreach (var ingredientPrefab in selectedIngredients)
        {
            var ingredient = ingredientPrefab.GetIngredient();
            ingredients.Add(ingredient);
            Debug.Log($"Ingredient: {ingredient.IngredientName} | Family: {ingredient.family} | SubFamily: {ingredient.subFamily}");
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
        Debug.Log($"Number of magic ingredients detected: {magicCount}");
        
        // Find matching recipe
        PotionRecipe matchedRecipe = null;
        matchedRecipe = FindBestRecipe(ingredients);
        
        if (matchedRecipe != null)
        {
            // Successfully created a potion!            
            Debug.Log($"=== POTION CREATED ===");
            Debug.Log($"Potion: {matchedRecipe.potionName}");
            Debug.Log($"Description: {matchedRecipe.description}");
            Debug.Log($"Level: {matchedRecipe.level}.{matchedRecipe.subLevel}");
            Debug.Log($"Duration: {matchedRecipe.duration} seconds");
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

    public PotionRecipe FindBestRecipe(List<Ingredient> ingredients)
    {
        PotionRecipe bestRecipe = null;
        int bestScore = -1;

        foreach (PotionRecipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                int score = recipe.CalculateScore(ingredients);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestRecipe = recipe;
                }
            }
        }

        return bestRecipe;
    }

    // Helper methods for creating requirements
    private CategoryRequirement MagicPlantRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Plant,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }

    private CategoryRequirement NaturalAnimalRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Animal,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }

    private CategoryRequirement NaturalMineralRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Mineral,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }

    private CategoryRequirement NaturalPlantRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Plant,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }

    private CategoryRequirement MagicAnimalRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Animal,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }

    private CategoryRequirement MagicMineralRequirement(int minQuantity = 1, int maxQuantity = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Mineral,
            minCount = minQuantity,
            maxCount = maxQuantity
        };
    }
}
