using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages display, animation and properties of a created potion
/// Combines Potion and AnimatedPotion functionalities
/// </summary>
public class AnimatedPotion : MonoBehaviour
{
    [Header("Potion Identity")]
    public string potionId = "";
    public string potionName = "";
    public string description = "";
    
    [Header("Potion Properties")]
    public int level = 1;
    public int subLevel = 1;
    public int duration = 30;
    public List<Ingredient> ingredients = new List<Ingredient>();
    
    [Header("Rendering (Auto-assigned)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    private PotionAnimationMapping currentMapping;
    
    void Awake()
    {
        // Auto-assign components
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Configure the potion with its ingredients, recipe and animation
    /// </summary>
    public void SetupPotion(List<Ingredient> usedIngredients, PotionRecipe recipe, PotionAnimationMapping mapping)
    {
        // Store recipe information
        if (recipe != null)
        {
            description = recipe.description;
            level = recipe.level;
            subLevel = recipe.subLevel;
            duration = recipe.duration;
        }
        
        // Store ingredients and generate ID
        ingredients = new List<Ingredient>(usedIngredients);
        potionId = PotionUtils.GeneratePotionId(usedIngredients);
        currentMapping = mapping;
        
        potionName = recipe?.potionName ?? $"Potion {mapping.MagicType}";
        SetupAnimation(mapping);
        
        Debug.Log($"Potion created: {potionName} (ID: {potionId})");
    }
    
    /// <summary>
    /// Configure the potion animation
    /// </summary>
    private void SetupAnimation(PotionAnimationMapping mapping)
    {
        if (mapping.animatorController != null)
        {
            // Assign animation controller
            if (animator == null)
                animator = gameObject.AddComponent<Animator>();
                
            animator.runtimeAnimatorController = mapping.animatorController;
            
            Debug.Log($"Controller assigned: {mapping.animatorController.name}");
        }
    }
    
    /// <summary>
    /// Called when potion is clicked/selected
    /// </summary>
    void OnMouseDown()
    {
        Debug.Log($"Potion clicked: {potionName} (ID: {potionId})");
        // TODO: Show potion details UI, allow to use/sell potion, etc.
    }
}
