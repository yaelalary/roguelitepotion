using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PotionRecipeBook", menuName = "PotionGame/Recipe Book")]
public class PotionRecipeBook : ScriptableObject
{
    [Header("All Available Recipes")]
    public List<PotionRecipe> recipes = new List<PotionRecipe>();
    
    [ContextMenu("Create Default Recipes")]
    public void CreateDefaultRecipes()
    {
        recipes.Clear();
        
        // === AJOUTEZ VOS RECETTES ICI ===
        // Exemples de syntaxe :
        
        // CreateRecipe("Nom de la potion", "Description", score_de_base, requirements...);
        
        // Exemples de requirements :
        // MagicRequirement(1, 1)                    // 1 ingrédient magique (n'importe lequel)
        // PlantRequirement(1, 1)                    // 1 plante naturelle
        // AnimalRequirement(1, 1)                   // 1 animal naturel
        // MineralRequirement(1, 1)                  // 1 minéral naturel
        
        // Exemple de recette simple :
        // CreateRecipe("Ma Potion", "Ma description", 100,
        //             PlantRequirement(1, 1),
        //             MagicRequirement(1, 1));
        
        Debug.Log($"Created {recipes.Count} recipes!");
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
    
    /// <summary>
    /// Find the best matching recipe for the given ingredients
    /// </summary>
    public PotionRecipe FindBestRecipe(List<Ingredient> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
            return null;
        
        // Check if there's at least one magic ingredient (game rule)
        bool hasMagicIngredient = false;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.family == IngredientFamily.Magic)
            {
                hasMagicIngredient = true;
                break;
            }
        }
        
        if (!hasMagicIngredient)
        {
            Debug.Log("Recipe search failed: No magic ingredient found!");
            return null;
        }
        
        // Find all matching recipes
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
            return null; // No valid potion possible
        }
        
        // Return the recipe with highest score potential
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
    
    private void CreateRecipe(string name, string description, int baseScore, params CategoryRequirement[] requirements)
    {
        var recipe = new PotionRecipe();
        recipe.potionName = name;
        recipe.description = description;
        recipe.baseScore = baseScore;
        recipe.categoryRequirements = new List<CategoryRequirement>(requirements);
        
        recipes.Add(recipe);
    }
    
    private CategoryRequirement MagicRequirement(int minCount, int maxCount)
    {
        return new CategoryRequirement
        {
            type = RequirementType.Family,
            targetFamily = IngredientFamily.Magic,
            minCount = minCount,
            maxCount = maxCount
        };
    }
    
    private CategoryRequirement PlantRequirement(int minCount, int maxCount)
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
    
    private CategoryRequirement MineralRequirement(int minCount, int maxCount)
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
    
    private CategoryRequirement AnimalRequirement(int minCount, int maxCount)
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
}
