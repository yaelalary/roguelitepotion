using UnityEngine;
using System.Collections.Generic;

// Force recompilation - ID system with initials
[System.Serializable]
public class PotionRecipe
{
    public string potionName = "";
    public string potionId = ""; // Unique ID based on ingredients
    public string description = "";
    
    [Header("Recipe Requirements")]
    public int minIngredients = 1;
    public int maxIngredients = 4;
    // Recipes can now have flexible magic ingredient requirements
    
    [Header("Category Requirements")]
    public List<CategoryRequirement> categoryRequirements = new List<CategoryRequirement>();
    
    [Header("Potion Properties")]
    public int level = 1;
    public int subLevel = 1;
    public int duration = 30;
    
    // Constructor to ensure lists are never null
    public PotionRecipe()
    {
        if (categoryRequirements == null)
            categoryRequirements = new List<CategoryRequirement>();
    }
    
    /// <summary>
    /// Checks if the given ingredients match this recipe
    /// </summary>
    public bool MatchesIngredients(List<Ingredient> ingredients)
    {
        // Safety check
        if (categoryRequirements == null)
            categoryRequirements = new List<CategoryRequirement>();
            
        // Check ingredient count
        if (ingredients.Count < minIngredients || ingredients.Count > maxIngredients)
            return false;
        
        // Check category requirements
        foreach (var requirement in categoryRequirements)
        {
            if (!requirement.IsSatisfiedBy(ingredients))
                return false;
        }
        
        return true;
    }
}

[System.Serializable]
public class CategoryRequirement
{
    public RequirementType type;
    public IngredientFamily targetFamily;
    public IngredientSubFamily targetSubFamily;
    public int minCount = 1;
    public int maxCount = 4;
    
    public bool IsSatisfiedBy(List<Ingredient> ingredients)
    {
        if (ingredients == null) return false;
        
        int count = 0; 
        foreach (var ingredient in ingredients)
        {
            bool matches = false;
            switch (type)
            {
                case RequirementType.Family:
                    matches = ingredient.family == targetFamily;
                    break;
                case RequirementType.SubFamily:
                    matches = ingredient.subFamily == targetSubFamily;
                    break;
                case RequirementType.FamilyAndSubFamily:
                    matches = ingredient.family == targetFamily && ingredient.subFamily == targetSubFamily;
                    break;
            }
            
            if (matches) count++;
        }
        
        return count >= minCount && count <= maxCount;
    }
}

public enum RequirementType
{
    Family,
    SubFamily, 
    FamilyAndSubFamily
}
