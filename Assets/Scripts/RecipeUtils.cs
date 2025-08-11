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
}
