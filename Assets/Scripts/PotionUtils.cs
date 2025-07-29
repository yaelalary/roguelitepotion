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
}
