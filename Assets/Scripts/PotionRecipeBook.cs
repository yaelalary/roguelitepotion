using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PotionRecipeBook : MonoBehaviour
{
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    
    void Awake()
    {
        CreateAllRecipes();
    }
    
    void CreateAllRecipes()
    {
        recipes.Clear();
        
        // === 1 INGREDIENT RECIPES ===
        CreateRecipe("Magic Plant Potion", "A basic potion made from one magic plant", 1, 1, 1,
                    MagicPlantRequirement(1, 1));
        
        CreateRecipe("Magic Animal Potion", "A basic potion made from one magic animal", 1, 1, 1,
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Mineral Potion", "A powerful potion made from one magic mineral", 1, 1, 1,
                    MagicMineralRequirement(1, 1));
        
        // === 2 INGREDIENT RECIPES - SAME TYPE ===
        CreateRecipe("Double Magic Plant Potion", "Enhanced plant magic", 2, 1, 4,
                    MagicPlantRequirement(2, 2));
        
        CreateRecipe("Double Magic Animal Potion", "Enhanced animal magic", 2, 1, 4,
                    MagicAnimalRequirement(2, 2));
        
        CreateRecipe("Double Magic Mineral Potion", "Very powerful mineral magic", 2, 1, 4,
                    MagicMineralRequirement(2, 2));
        
        // === 2 INGREDIENT RECIPES - MIXED MAGIC ===
        CreateRecipe("Magic Plant & Animal Potion", "Mixed essence of plant and animal", 2, 1, 3,
                    MagicPlantRequirement(1, 1),
                    MagicAnimalRequirement(1, 1));
        
        CreateRecipe("Magic Plant & Mineral Potion", "Mixed essence of plant and mineral", 2, 1, 3,
                    MagicPlantRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        CreateRecipe("Magic Animal & Mineral Potion", "Mixed essence of animal and mineral", 2, 1, 3,
                    MagicAnimalRequirement(1, 1),
                    MagicMineralRequirement(1, 1));
        
        // === 2 INGREDIENT RECIPES - MAGIC + NATURAL ===
        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicPlantRequirement(1, 1),
                    NaturalPlantRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicPlantRequirement(1, 1),
                    NaturalAnimalRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicPlantRequirement(1, 1),
                    NaturalMineralRequirement(1, 1));
        
        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicAnimalRequirement(1, 1),
                    NaturalPlantRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicAnimalRequirement(1, 1),
                    NaturalAnimalRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicAnimalRequirement(1, 1),
                    NaturalMineralRequirement(1, 1));
        
        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicMineralRequirement(1, 1),
                    NaturalPlantRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicMineralRequirement(1, 1),
                    NaturalAnimalRequirement(1, 1));

        CreateRecipe("Hybrid Potion", "Fusion", 2, 1, 2,
                    MagicMineralRequirement(1, 1),
                    NaturalMineralRequirement(1, 1));
                    
        Debug.Log($"Created {recipes.Count} recipes!");
    }
    
    /// <summary>
    /// Find the best matching recipe for the given ingredients
    /// </summary>
    public PotionRecipe FindBestRecipe(List<Ingredient> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
            return null;
        
        // Find all matching recipes (MatchesIngredients will check for magic ingredient)
        List<PotionRecipe> matchingRecipes = new List<PotionRecipe>();
        
        foreach (var recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                matchingRecipes.Add(recipe);
            }
        }
        
        // If no recipe matches, the potion is invalid
        if (matchingRecipes.Count == 0)
        {
            Debug.Log("Recipe search failed: No matching recipe found!");
            return null; // No valid potion possible
        }
        
        // Return the recipe with highest level (level.subLevel)
        return matchingRecipes.OrderByDescending(r => r.CalculateScore(ingredients)).First();
    }
    
    /// <summary>
    /// Check if the ingredients can make any valid potion
    /// </summary>
    public bool CanMakePotion(List<Ingredient> ingredients)
    {
        return FindBestRecipe(ingredients) != null;
    }
    
    /// <summary>
    /// Get all possible recipes that these ingredients could contribute to
    /// </summary>
    public List<PotionRecipe> GetPossibleRecipes(List<Ingredient> ingredients)
    {
        List<PotionRecipe> possibleRecipes = new List<PotionRecipe>();
        
        foreach (var recipe in recipes)
        {
            // Check if current ingredients could be part of this recipe
            // (even if not complete yet)
            if (CouldContributeToRecipe(ingredients, recipe))
            {
                possibleRecipes.Add(recipe);
            }
        }
        
        return possibleRecipes;
    }
    
    private bool CouldContributeToRecipe(List<Ingredient> ingredients, PotionRecipe recipe)
    {
        // Basic checks
        if (ingredients.Count > recipe.maxIngredients)
            return false;
        
        // Check if adding more ingredients could satisfy the recipe
        // This is a simplified check - you might want to make it more sophisticated
        foreach (var requirement in recipe.categoryRequirements)
        {
            int currentCount = 0;
            foreach (var ingredient in ingredients)
            {
                bool matches = false;
                switch (requirement.type)
                {
                    case RequirementType.Family:
                        matches = ingredient.family == requirement.targetFamily;
                        break;
                    case RequirementType.SubFamily:
                        matches = ingredient.subFamily == requirement.targetSubFamily;
                        break;
                    case RequirementType.FamilyAndSubFamily:
                        matches = ingredient.family == requirement.targetFamily && 
                                 ingredient.subFamily == requirement.targetSubFamily;
                        break;
                }
                if (matches) currentCount++;
            }
            
            // If we already exceed the max, this recipe is impossible
            if (currentCount > requirement.maxCount)
                return false;
        }
        
        return true;
    }
    
    // === RECIPE CREATION HELPERS ===
    
    private void CreateRecipe(string name, string description, int level, int subLevel, int duration, params CategoryRequirement[] requirements)
    {
        var recipe = new PotionRecipe();
        recipe.potionName = name;
        recipe.description = description;
        recipe.level = level;
        recipe.subLevel = subLevel;
        recipe.duration = duration;
        recipe.categoryRequirements = new List<CategoryRequirement>(requirements);
        
        recipes.Add(recipe);
    }
    
    // === NATURAL INGREDIENT HELPERS ===
    
    private CategoryRequirement NaturalPlantRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Plant,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    private CategoryRequirement NaturalAnimalRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Animal,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    private CategoryRequirement NaturalMineralRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = IngredientSubFamily.Mineral,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    // === MAGIC INGREDIENT HELPERS ===
    
    private CategoryRequirement MagicPlantRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Plant,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    private CategoryRequirement MagicAnimalRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Animal,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    private CategoryRequirement MagicMineralRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = IngredientSubFamily.Mineral,
            minCount = minCount,
            maxCount = maxCount
        };
    }
}
