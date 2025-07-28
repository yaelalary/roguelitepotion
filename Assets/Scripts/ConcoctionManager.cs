using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    public TextMeshProUGUI recipeNameText;
    
    [Header("Game References")]
    public Basket2D basket;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    private PotionRecipe currentSelectedRecipe = null; // Currently selected recipe

    void Start()
    {
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(false);
            concoctButton.onClick.AddListener(ConcoctPotion);
        }
        
        // Initialize recipe display text
        UpdateRecipeDisplay();
        
        CreateAllRecipes();
    }
    
    void CreateAllRecipes()
    {
        recipes.Clear();
        
        // Recipe priority: most specific (uses most ingredients) to most general
        
        // 2 IDENTICAL INGREDIENTS - Most specific recipes
        CreateRecipe("Double Magic Plant Potion", "Enhanced plant magic", 3, 1, 60,
                    MagicPlantRequirement(2, 2));
        
        CreateRecipe("Double Magic Animal Potion", "Enhanced animal magic", 3, 1, 60,
                    MagicAnimalRequirement(2, 2));
        
        CreateRecipe("Double Magic Mineral Potion", "Very powerful mineral magic", 3, 1, 90,
                    MagicMineralRequirement(2, 2));
        
        // 2 DIFFERENT MAGIC INGREDIENTS
        CreateRecipe("Magic Plant & Animal Potion", "Mixed essence of plant and animal", 2, 2, 45,
                    MagicPlantRequirement(1, 1),
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Plant & Mineral Potion", "Mixed essence of plant and mineral", 2, 2, 45,
                    MagicPlantRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        CreateRecipe("Magic Animal & Mineral Potion", "Mixed essence of animal and mineral", 2, 2, 45,
                    MagicAnimalRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        // 2 DIFFERENT INGREDIENTS - Magic + Natural
        CreateRecipe("Hybrid Plant Potion", "Magic and natural plant fusion", 2, 1, 30,
                    MagicPlantRequirement(1, 1),
                    NaturalPlantRequirement(1, 1));
        
        CreateRecipe("Hybrid Animal Potion", "Magic and natural animal fusion", 2, 1, 30,
                    MagicAnimalRequirement(1, 1),
                    NaturalAnimalRequirement(1, 1));
        
        CreateRecipe("Hybrid Mineral Potion", "Magic and natural mineral fusion", 2, 1, 30,
                    MagicMineralRequirement(1, 1),
                    NaturalMineralRequirement(1, 1));
        
        // 1 INGREDIENT - General fallback recipes
        CreateRecipe("Magic Plant Potion", "A basic potion made from one magic plant", 1, 1, 30,
                    MagicPlantRequirement(1, 1));
        
        CreateRecipe("Magic Animal Potion", "A basic potion made from one magic animal", 1, 1, 30,
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Mineral Potion", "A powerful potion made from one magic mineral", 1, 2, 60,
                    MagicMineralRequirement(1, 1));

        Debug.Log($"Created {recipes.Count} recipes in specificity order!");
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
            UpdateRecipeSelection(); // Update selected recipe
            UpdateButtonVisibility();
        }
    }
    
    public void RemoveSelectedIngredient(IngredientPrefab ingredient)
    {
        selectedIngredients.Remove(ingredient);
        UpdateRecipeSelection(); // Update selected recipe
        UpdateButtonVisibility();
    }
    
    void UpdateButtonVisibility()
    {
        if (concoctButton != null) concoctButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }
    
    void UpdateRecipeSelection()
    {
        // Find the first recipe (most specific) that matches current ingredients
        currentSelectedRecipe = FindFirstMatchingRecipe();
        UpdateRecipeDisplay();
    }
    
    void UpdateRecipeDisplay()
    {
        if (recipeNameText != null)
        {
            if (currentSelectedRecipe != null)
            {
                recipeNameText.text = currentSelectedRecipe.potionName;
            }
            else
            {
                recipeNameText.text = selectedIngredients.Count > 0 ? "No valid recipe" : "Select ingredients";
            }
        }
    }
    
    PotionRecipe FindFirstMatchingRecipe()
    {
        if (selectedIngredients.Count == 0) return null;
        
        // Convert selected ingredients to ingredient list
        List<Ingredient> ingredients = new List<Ingredient>();
        foreach (var ingredientPrefab in selectedIngredients)
        {
            ingredients.Add(ingredientPrefab.GetIngredient());
        }
        
        // Return the FIRST recipe that matches (most specific due to ordering)
        foreach (PotionRecipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                return recipe;
            }
        }
        
        return null; // No matching recipe found
    }

    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0 || currentSelectedRecipe == null) return;
        
        Debug.Log("=== CONCOCTING POTION ===");
        Debug.Log($"Selected Recipe: {currentSelectedRecipe.potionName}");
        Debug.Log($"Description: {currentSelectedRecipe.description}");
        Debug.Log($"Level: {currentSelectedRecipe.level}.{currentSelectedRecipe.subLevel}");
        Debug.Log($"Duration: {currentSelectedRecipe.duration} seconds");
        Debug.Log($"Ingredients used:");
        
        foreach (var ingredientPrefab in selectedIngredients)
        {
            var ingredient = ingredientPrefab.GetIngredient();
            Debug.Log($"- {ingredient.IngredientName} ({ingredient.family}, {ingredient.subFamily})");
        }
        
        // TODO: Add potion to inventory, update score, show success animation
        
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
        currentSelectedRecipe = null; // Reset selected recipe
        UpdateRecipeDisplay();
        UpdateButtonVisibility();
    }
    
    public List<IngredientPrefab> GetSelectedIngredients()
    {
        return new List<IngredientPrefab>(selectedIngredients);
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
