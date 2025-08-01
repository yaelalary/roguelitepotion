using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class for potion-related operations and calculations
/// Contains static methods for ID generation, validation, and other potion utilities
/// </summary>
public static class PotionUtils
{
    /// <summary>
    /// Generates a unique ID based on the ingredients used in the potion
    /// Format: Uses initials - "MP1" (Magic Plant x1), "MP2NM1" (Magic Plant x2 + Natural Mineral x1)
    /// Family: M=Magic, N=Natural | SubFamily: P=Plant, A=Animal, M=Mineral
    /// </summary>
    public static string GeneratePotionId(List<Ingredient> ingredients)
    {    
        // Count occurrences of each ingredient type
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        
        foreach (var ingredient in ingredients)
        {
            // Generate initials: Family + SubFamily
            string familyInitial = GetInitial(ingredient.family.ToString());
            string subFamilyInitial = GetInitial(ingredient.subFamily.ToString());

            string key = familyInitial + subFamilyInitial;
            if (ingredientCounts.ContainsKey(key))
                ingredientCounts[key]++;
            else
                ingredientCounts[key] = 1;
        }
        
        // Sort keys alphabetically for consistent ID generation
        var sortedKeys = new List<string>(ingredientCounts.Keys);
        sortedKeys.Sort();
        
        // Build the ID string
        string potionId = "";
        foreach (string key in sortedKeys)
        {
            potionId += key + ingredientCounts[key].ToString();
        }
        
        return potionId;
    }

    /// <summary>
    /// Get the initial letter for a string
    /// </summary>
    public static string GetInitial(string str)
    {
        if (string.IsNullOrEmpty(str))
            return "?";
        return str[0].ToString().ToUpper();
    }
    
    /// <summary>
    /// Generate all possible potion IDs based on GameConstants
    /// </summary>
    public static List<string> GetAllPossiblePotionIds()
    {
        List<string> allIds = new List<string>();
        
        // Available families and subfamilies
        string[] families = { GameConstants.MAGIC_FAMILY, GameConstants.NATURAL_FAMILY };
        string[] subFamilies = { GameConstants.PLANT_SUBFAMILY, GameConstants.ANIMAL_SUBFAMILY, GameConstants.MINERAL_SUBFAMILY };
        
        // Generate all possible IDs for 1 to 4 ingredients
        for (int totalIngredients = 1; totalIngredients <= GameConstants.MAX_INGREDIENTS_PER_POTION; totalIngredients++)
        {
            // Generate all possible combinations
            GenerateCombinations(families, subFamilies, totalIngredients, allIds);
        }
        
        // Remove duplicates and sort
        var uniqueIds = new HashSet<string>(allIds);
        var sortedIds = new List<string>(uniqueIds);
        sortedIds.Sort();
        
        return sortedIds;
    }
    
    /// <summary>
    /// Recursively generate all ingredient combinations
    /// </summary>
    private static void GenerateCombinations(string[] families, string[] subFamilies, int targetCount, List<string> result)
    {
        // Create all family+subfamily combinations
        List<string> ingredientTypes = new List<string>();
        foreach (string family in families)
        {
            foreach (string subFamily in subFamilies)
            {
                string familyInitial = GetInitial(family);
                string subFamilyInitial = GetInitial(subFamily);
                ingredientTypes.Add(familyInitial + subFamilyInitial);
            }
        }
        
        // Generate all possible combinations with repetition
        GeneratePermutationsWithRepetition(ingredientTypes, targetCount, new List<string>(), result);
    }
    
    /// <summary>
    /// Generate all permutations with repetition
    /// </summary>
    private static void GeneratePermutationsWithRepetition(List<string> types, int remaining, List<string> current, List<string> result)
    {
        if (remaining == 0)
        {
            // Create ID from current combination
            var counts = new Dictionary<string, int>();
            foreach (string type in current)
            {
                counts[type] = counts.ContainsKey(type) ? counts[type] + 1 : 1;
            }
            
            // Sort and build ID
            var sortedKeys = new List<string>(counts.Keys);
            sortedKeys.Sort();
            
            string id = "";
            foreach (string key in sortedKeys)
            {
                id += key + counts[key].ToString();
            }
            
            if (!result.Contains(id))
                result.Add(id);
            return;
        }
        
        foreach (string type in types)
        {
            current.Add(type);
            GeneratePermutationsWithRepetition(types, remaining - 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
}
