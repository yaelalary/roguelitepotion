using UnityEngine;

/// <summary>
/// Constants for all ingredient names in the game.
/// Use these constants instead of hardcoding names in code.
/// </summary>
public static class IngredientNames
{
    // Magic Ingredients - Plants
    public const string ILLUSION_HERB = "illusion_herb";
    public const string MIST_SEED = "mist_seed";
    
    // Magic Ingredients - Animals
    public const string PHOENIX_FEATHER = "phoenix_feather";
    public const string UNICORN_HAIR = "unicorn_hair";
    
    // Magic Ingredients - Minerals
    public const string DREAMSTONE = "dreamstone";
    public const string STARDUST = "stardust";
    
    // Natural Ingredients - Plants
    public const string FLOWER = "flower";
    public const string LEAF = "leaf";

    // Natural Ingredients - Animals
    public const string CAT_HAIR = "cat_hair";
    public const string SNAKE_VENOM = "snake_venom";

    // Natural Ingredients - Minerals
    public const string AMETHYST = "amethyst";
    public const string SULFUR = "sulfur";

    /// <summary>
    /// Returns all ingredient names in an array
    /// Order: Magic (Plant, Animal, Mineral), then Natural (Plant, Animal, Mineral)
    /// </summary>
    public static string[] GetAllIngredientNames()
    {
        return new string[]
        {
            ILLUSION_HERB, MIST_SEED,
            PHOENIX_FEATHER, UNICORN_HAIR,
            DREAMSTONE, STARDUST,
            FLOWER, LEAF,
            CAT_HAIR, SNAKE_VENOM,
            AMETHYST, SULFUR
        };
    }
    
    /// <summary>
    /// Checks if an ingredient name is valid
    /// </summary>
    public static bool IsValidIngredientName(string name)
    {
        string[] allNames = GetAllIngredientNames();
        for (int i = 0; i < allNames.Length; i++)
        {
            if (allNames[i] == name) return true;
        }
        return false;
    }
    
    /// <summary>
    /// Returns a formatted ingredient name for display
    /// </summary>
    public static string GetDisplayName(string ingredientName)
    {
        switch (ingredientName)
        {
            case FLOWER: return "Flower";
            case LEAF: return "Leaf";
            case ILLUSION_HERB: return "Illusion Herb";
            case MIST_SEED: return "Mist Seed";
            case CAT_HAIR: return "Cat Hair";
            case SNAKE_VENOM: return "Snake Venom";
            case PHOENIX_FEATHER: return "Phoenix Feather";
            case UNICORN_HAIR: return "Unicorn Hair";
            case AMETHYST: return "Amethyst";
            case DREAMSTONE: return "Dreamstone";
            case STARDUST: return "Stardust";
            case SULFUR: return "Sulfur";
            default: return ingredientName;
        }
    }
}
