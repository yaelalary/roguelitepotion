using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Associates a magic potion type to its animation controller
/// Based on the magic ingredient types used (natural ingredients don't count)
/// </summary>
[System.Serializable]
public class PotionAnimationMapping
{
    [Header("Magic Type (Auto-generated)")]
    [SerializeField] private string _magicType;             // Auto-generated, read-only in Inspector
    [SerializeField] private string _description;           // Auto-generated description
    
    [Header("Animation (Assign manually)")]
    public RuntimeAnimatorController animatorController;    // User assigns this
    
    [Header("Visual (Optional)")]
    public Sprite defaultSprite;                           // Optional fallback sprite
    
    // Read-only properties for external access
    public string MagicType => _magicType;
    public string Description => _description;
    
    // Constructor for script initialization
    public PotionAnimationMapping(string magicType, string description)
    {
        _magicType = magicType;
        _description = description;
    }
    
    // Parameterless constructor for Unity serialization
    public PotionAnimationMapping() { }
}

/// <summary>
/// ScriptableObject that contains the 7 possible magic potion types
/// </summary>
[CreateAssetMenu(fileName = "PotionAnimationDatabase", menuName = "PotionGame/Potion Animation Database")]
public class PotionAnimationDatabase : ScriptableObject
{
    [Header("Magic Potion Types (7 controllers)")]
    public PotionAnimationMapping[] magicTypes = new PotionAnimationMapping[7];
    
    /// <summary>
    /// Initialize the 7 possible magic potion types
    /// </summary>
    public void InitializeMagicTypes()
    {
        magicTypes = new PotionAnimationMapping[7]
        {
            new PotionAnimationMapping("Plant", "Magic Plant only"),
            new PotionAnimationMapping("Animal", "Magic Animal only"),
            new PotionAnimationMapping("Mineral", "Magic Mineral only"),
            new PotionAnimationMapping("Plant+Animal", "Magic Plant + Animal"),
            new PotionAnimationMapping("Plant+Mineral", "Magic Plant + Mineral"),
            new PotionAnimationMapping("Animal+Mineral", "Magic Animal + Mineral"),
            new PotionAnimationMapping("Plant+Animal+Mineral", "All 3 magic types")
        };
        
        Debug.Log("Initialized 7 magic potion types!");
    }
    
    /// <summary>
    /// Determine the magic potion type based on ingredients
    /// </summary>
    public PotionAnimationMapping GetMappingForIngredients(List<Ingredient> ingredients)
    {
        bool hasPlant = false;
        bool hasAnimal = false;
        bool hasMineral = false;
        
        // Analyze only magic ingredients
        foreach (var ingredient in ingredients)
        {
            if (ingredient.family == IngredientFamily.Magic)
            {
                if (ingredient.subFamily == IngredientSubFamily.Plant) hasPlant = true;
                if (ingredient.subFamily == IngredientSubFamily.Animal) hasAnimal = true;
                if (ingredient.subFamily == IngredientSubFamily.Mineral) hasMineral = true;
            }
        }
        
        // Determine controller index (0-6)
        int index = GetControllerIndex(hasPlant, hasAnimal, hasMineral);
        
        if (index >= 0 && index < magicTypes.Length && magicTypes[index] != null)
        {
            return magicTypes[index];
        }
        
        Debug.LogWarning("No magic potion type found for these ingredients!");
        return null;
    }
    
    /// <summary>
    /// Determine controller index based on present magic types
    /// </summary>
    private int GetControllerIndex(bool hasPlant, bool hasAnimal, bool hasMineral)
    {
        // Logic according to specifications:
        if (hasPlant && hasAnimal && hasMineral) return 6; // Plant+Animal+Mineral (controller 7)
        if (hasAnimal && hasMineral) return 5;             // Animal+Mineral (controller 6)
        if (hasPlant && hasMineral) return 4;              // Plant+Mineral (controller 5)
        if (hasPlant && hasAnimal) return 3;               // Plant+Animal (controller 4)
        if (hasMineral) return 2;                          // Mineral only (controller 3)
        if (hasAnimal) return 1;                           // Animal only (controller 2)
        if (hasPlant) return 0;                            // Plant only (controller 1)
        
        return -1; // No magic ingredient (should not happen)
    }
}
