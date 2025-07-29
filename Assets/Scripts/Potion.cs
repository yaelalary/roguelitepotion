using UnityEngine;

/// <summary>
/// Component for individual potion instances created on shelves
/// Contains the unique ID and recipe information
/// </summary>
public class Potion : MonoBehaviour
{
    [Header("Potion Identity")]
    public string potionId = ""; // Unique ID based on ingredients
    public string potionName = "";
    public string description = "";
    
    [Header("Potion Properties")]
    public int level = 1;
    public int subLevel = 1;
    public int duration = 30;
    
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    
    /// <summary>
    /// Initialize this potion with recipe data and unique ID
    /// </summary>
    public void SetRecipe(PotionRecipe recipe, string uniqueId)
    {
        potionId = uniqueId;
        potionName = recipe.potionName;
        description = recipe.description;
        level = recipe.level;
        subLevel = recipe.subLevel;
        duration = recipe.duration;
    }
    
    /// <summary>
    /// Called when potion is clicked/selected
    /// </summary>
    void OnMouseDown()
    {
        // TODO: Show potion details UI, allow to use/sell potion, etc.
    }
}
