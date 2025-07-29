using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class for recipe-related operations
/// Contains helper methods for recipe creation and validation
/// </summary>
public static class RecipeUtils
{
    /// <summary>
    /// Create a category requirement for magic ingredients of a specific subfamily
    /// </summary>
    public static CategoryRequirement CreateMagicRequirement(IngredientSubFamily subFamily, int minCount = 1, int maxCount = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Magic,
            targetSubFamily = subFamily,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    /// <summary>
    /// Create a category requirement for natural ingredients of a specific subfamily
    /// </summary>
    public static CategoryRequirement CreateNaturalRequirement(IngredientSubFamily subFamily, int minCount = 1, int maxCount = 1)
    {
        return new CategoryRequirement
        {
            type = RequirementType.FamilyAndSubFamily,
            targetFamily = IngredientFamily.Natural,
            targetSubFamily = subFamily,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    /// <summary>
    /// Create a magic plant requirement (shorthand)
    /// </summary>
    public static CategoryRequirement MagicPlant(int minCount = 1, int maxCount = 1)
    {
        return CreateMagicRequirement(IngredientSubFamily.Plant, minCount, maxCount);
    }
    
    /// <summary>
    /// Create a magic animal requirement (shorthand)
    /// </summary>
    public static CategoryRequirement MagicAnimal(int minCount = 1, int maxCount = 1)
    {
        return CreateMagicRequirement(IngredientSubFamily.Animal, minCount, maxCount);
    }
    
    /// <summary>
    /// Create a magic mineral requirement (shorthand)
    /// </summary>
    public static CategoryRequirement MagicMineral(int minCount = 1, int maxCount = 1)
    {
        return CreateMagicRequirement(IngredientSubFamily.Mineral, minCount, maxCount);
    }
    
    /// <summary>
    /// Create a natural plant requirement (shorthand)
    /// </summary>
    public static CategoryRequirement NaturalPlant(int minCount = 1, int maxCount = 1)
    {
        return CreateNaturalRequirement(IngredientSubFamily.Plant, minCount, maxCount);
    }
    
    /// <summary>
    /// Create a natural animal requirement (shorthand)
    /// </summary>
    public static CategoryRequirement NaturalAnimal(int minCount = 1, int maxCount = 1)
    {
        return CreateNaturalRequirement(IngredientSubFamily.Animal, minCount, maxCount);
    }
    
    /// <summary>
    /// Create a natural mineral requirement (shorthand)
    /// </summary>
    public static CategoryRequirement NaturalMineral(int minCount = 1, int maxCount = 1)
    {
        return CreateNaturalRequirement(IngredientSubFamily.Mineral, minCount, maxCount);
    }
    
    /// <summary>
    /// Validate if a recipe has valid requirements
    /// </summary>
    public static bool IsValidRecipe(PotionRecipe recipe)
    {
        if (recipe == null) return false;
        if (string.IsNullOrEmpty(recipe.potionName)) return false;
        if (recipe.categoryRequirements == null || recipe.categoryRequirements.Count == 0) return false;
        if (recipe.level < GameConstants.MIN_POTION_LEVEL || recipe.level > GameConstants.MAX_POTION_LEVEL) return false;
        if (recipe.duration < GameConstants.MIN_POTION_DURATION || recipe.duration > GameConstants.MAX_POTION_DURATION) return false;
        
        return true;
    }
    
    /// <summary>
    /// Get the complexity score of a recipe (higher = more complex)
    /// </summary>
    public static int GetRecipeComplexity(PotionRecipe recipe)
    {
        if (recipe?.categoryRequirements == null) return 0;
        
        int complexity = 0;
        foreach (var requirement in recipe.categoryRequirements)
        {
            complexity += requirement.minCount;
        }
        
        return complexity;
    }
}
