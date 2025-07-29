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
