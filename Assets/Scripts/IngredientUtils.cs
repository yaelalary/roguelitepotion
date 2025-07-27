using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utilities for ingredient and recipe management
/// </summary>
public static class IngredientUtils
{
    /// <summary>
    /// Finds an ingredient by name in an ingredient array
    /// </summary>
    public static Ingredient FindIngredientByName(Ingredient[] ingredients, string name)
    {
        foreach (var ingredient in ingredients)
        {
            if (ingredient.IngredientName == name)
            {
                return ingredient;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Counts how many ingredients of a given family are in a list
    /// </summary>
    public static int CountIngredientsByFamily(List<Ingredient> ingredients, IngredientFamily family)
    {
        int count = 0;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.family == family) count++;
        }
        return count;
    }
    
    /// <summary>
    /// Counts how many ingredients of a given subfamily are in a list
    /// </summary>
    public static int CountIngredientsBySubFamily(List<Ingredient> ingredients, IngredientSubFamily subFamily)
    {
        int count = 0;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.subFamily == subFamily) count++;
        }
        return count;
    }
    
    /// <summary>
    /// Checks if a list of ingredients contains a specific ingredient by name
    /// </summary>
    public static bool ContainsIngredient(List<Ingredient> ingredients, string ingredientName)
    {
        foreach (var ingredient in ingredients)
        {
            if (ingredient.IngredientName == ingredientName) return true;
        }
        return false;
    }
    
    /// <summary>
    /// Returns all ingredient names from a list
    /// </summary>
    public static List<string> GetIngredientNames(List<Ingredient> ingredients)
    {
        List<string> names = new List<string>();
        foreach (var ingredient in ingredients)
        {
            names.Add(ingredient.IngredientName);
        }
        return names;
    }
    
    /// <summary>
    /// Checks if two ingredient lists are identical (same content, order doesn't matter)
    /// </summary>
    public static bool AreIngredientListsEqual(List<Ingredient> list1, List<Ingredient> list2)
    {
        if (list1.Count != list2.Count) return false;
        
        List<string> names1 = GetIngredientNames(list1);
        List<string> names2 = GetIngredientNames(list2);
        
        names1.Sort();
        names2.Sort();
        
        for (int i = 0; i < names1.Count; i++)
        {
            if (names1[i] != names2[i]) return false;
        }
        
        return true;
    }
}
