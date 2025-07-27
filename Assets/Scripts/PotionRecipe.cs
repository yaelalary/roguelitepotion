using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PotionRecipe
{
    public string potionName;
    public string description;
    public Sprite potionIcon;
    
    [Header("Recipe Requirements")]
    public int minIngredients = 1;
    public int maxIngredients = 4;
    public bool requiresMagicIngredient = true;
    
    [Header("Category Requirements")]
    public List<CategoryRequirement> categoryRequirements = new List<CategoryRequirement>();
    
    [Header("Potion Effects")]
    public int baseScore = 10;
    public float scoreMultiplier = 1.0f;
    
    /// <summary>
    /// Checks if the given ingredients match this recipe
    /// </summary>
    public bool MatchesIngredients(List<Ingredient> ingredients)
    {
        // Check ingredient count
        if (ingredients.Count < minIngredients || ingredients.Count > maxIngredients)
            return false;
        
        // Check if at least one magic ingredient is required
        if (requiresMagicIngredient)
        {
            bool hasMagicIngredient = false;
            foreach (var ingredient in ingredients)
            {
                if (ingredient.family == IngredientFamily.Magic)
                {
                    hasMagicIngredient = true;
                    break;
                }
            }
            if (!hasMagicIngredient) return false;
        }
        
        // Check category requirements
        foreach (var requirement in categoryRequirements)
        {
            if (!requirement.IsSatisfiedBy(ingredients))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Calculate the score for this potion based on ingredients
    /// </summary>
    public int CalculateScore(List<Ingredient> ingredients)
    {
        float score = baseScore;
        
        // Bonus for using more ingredients
        score += (ingredients.Count - 1) * 5;
        
        // Apply multiplier
        score *= scoreMultiplier;
        
        return Mathf.RoundToInt(score);
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
