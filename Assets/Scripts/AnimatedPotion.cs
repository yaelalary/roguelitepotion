using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

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
    
    [Header("Tooltip")]
    [SerializeField] private PotionTooltip tooltip;
    
    [Header("Replacement Mode")]
    [SerializeField] private bool isInReplacementMode = false;
    
    [Header("Hover Animation")]
    [SerializeField] private float hoverOffset = 0.2f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    
    [Header("Tooltip Collider")]
    [SerializeField] private float tooltipExtension = 4.0f; // How far to extend collider for tooltip area
    
    [Header("Replacement Mode Animation")]
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private float shakeDuration = 1f;
    
    private PotionAnimationMapping currentMapping;
    private ConcoctionManager concoctionManager;
    private Vector3 originalPosition;
    private bool isHovering = false;
    private BoxCollider2D potionCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Tween shakeAnimation; // Store the shake animation reference
    
    void Awake()
    {
        // Auto-assign components from PotionSprite child
        Transform potionSpriteChild = transform.Find("PotionSprite");

        spriteRenderer = potionSpriteChild.GetComponent<SpriteRenderer>();
        animator = potionSpriteChild.GetComponent<Animator>();
        
        if (tooltip == null)
            tooltip = GetComponent<PotionTooltip>();
            
        concoctionManager = FindObjectOfType<ConcoctionManager>();
        originalPosition = transform.position;
        SetupTooltipCollider();
    }
    
    void OnDestroy()
    {
        // Clean up any active shake animation
        StopShakeAnimation();
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
        SetupTooltip();
        
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
    /// Setup the tooltip component
    /// </summary>
    private void SetupTooltip()
    {
        if (tooltip == null)
        {
            tooltip = gameObject.AddComponent<PotionTooltip>();
        }
        
        tooltip.Initialize(this);
    }
    
    /// <summary>
    /// Setup the collider to extend towards tooltip area
    /// </summary>
    private void SetupTooltipCollider()
    {
        // Get or add BoxCollider2D
        potionCollider = GetComponent<BoxCollider2D>();
        if (potionCollider == null)
        {
            potionCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Store original collider properties
        originalColliderSize = potionCollider.size;
        originalColliderOffset = potionCollider.offset;
        
        Debug.Log($"Original collider - Size: {originalColliderSize}, Offset: {originalColliderOffset}");
    }
    
    /// <summary>
    /// Extend collider to include tooltip area
    /// </summary>
    private void ExtendColliderForTooltip()
    {
        if (potionCollider == null) return;
        
        // Extend collider to the left to cover tooltip area
        Vector2 extendedSize = new Vector2(originalColliderSize.x + tooltipExtension, originalColliderSize.y);
        Vector2 extendedOffset = new Vector2(originalColliderOffset.x - tooltipExtension/2, originalColliderOffset.y);
        
        potionCollider.size = extendedSize;
        potionCollider.offset = extendedOffset;
        
        Debug.Log($"Extended collider - Size: {extendedSize}, Offset: {extendedOffset}");
    }    /// <summary>
    /// Reset collider to original size
    /// </summary>
    private void ResetColliderSize()
    {
        if (potionCollider == null) return;
        
        potionCollider.size = originalColliderSize;
        potionCollider.offset = originalColliderOffset;
        
        Debug.Log($"Reset collider - Size: {originalColliderSize}, Offset: {originalColliderOffset}");
    }
    
    /// <summary>
    /// Called when mouse enters the potion collider
    /// </summary>
    void OnMouseEnter()
    {
        tooltip.ShowTooltip();
        
        // Extend collider to include tooltip area so tooltip doesn't disappear
        ExtendColliderForTooltip();

        // Animate upward movement like ingredients
        if (!isHovering)
        {
            isHovering = true;
            
            // Stop shake animation during hover
            if (isInReplacementMode)
            {
                StopShakeAnimation();
            }
            
            transform.DOKill(); // Stop any existing animation
            originalPosition = transform.position;
            Vector3 targetPosition = originalPosition + Vector3.up * hoverOffset;
            transform.DOMove(targetPosition, hoverDuration).SetEase(hoverEase);
        }
    }
    
    /// <summary>
    /// Called when mouse exits the potion collider
    /// </summary>
    void OnMouseExit()
    {
        tooltip.HideTooltip();

        // Don't do anything if colliders are disabled (destruction in progress)
        if (potionCollider != null && !potionCollider.enabled)
        {
            Debug.Log("Collider disabled, skipping OnMouseExit animation");
            return;
        }
        
        // Reset collider to original size
        ResetColliderSize();
        
        // Return to original position
        if (isHovering)
        {
            isHovering = false;
            transform.DOKill(); // Stop any existing animation
            
            // Animate back to original position, then restart shake if needed
            transform.DOMove(originalPosition, hoverDuration).SetEase(hoverEase)
                .OnComplete(() => {
                    // Restart shake animation if still in replacement mode
                    if (isInReplacementMode)
                    {
                        StartShakeAnimation();
                    }
                });
        }
    }
    
    /// <summary>
    /// Called when potion is clicked/selected
    /// </summary>
    void OnMouseDown()
    {
        if (isInReplacementMode)
        {
            // In replacement mode, select this potion for replacement
            concoctionManager.OnPotionSelectedForReplacement(this);
        }
        else
        {
            // Normal click behavior
            Debug.Log($"Potion clicked: {potionName} (ID: {potionId})");
            // TODO: Show potion details UI, allow to use/sell potion, etc.
        }
    }
    
    /// <summary>
    /// Enable or disable potion interactions (collider)
    /// </summary>
    public void SetInteractionsEnabled(bool enabled)
    {
        if (potionCollider != null) potionCollider.enabled = enabled;    
    }
    
    /// <summary>
    /// Enable or disable replacement mode for this potion
    /// </summary>
    public void SetReplacementMode(bool enabled)
    {
        isInReplacementMode = enabled;
        
        // Visual feedback for replacement mode with shake animation
        if (enabled)
        {
            // Only start shake animation if not currently hovering
            if (!isHovering)
            {
                StartShakeAnimation();
            }
            // If hovering, the shake will start when hover ends
        }
        else
        {
            // Stop shake animation
            StopShakeAnimation();
        }
        
        Debug.Log($"Potion {potionName} replacement mode: {(enabled ? "ON" : "OFF")}");
    }
    
    /// <summary>
    /// Start the shake animation for replacement mode
    /// </summary>
    private void StartShakeAnimation()
    {
        // Stop any existing shake animation
        StopShakeAnimation();
        
        // Create infinite shake animation
        shakeAnimation = transform.DOShakePosition(shakeDuration, shakeIntensity, 10, 90, false, true)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutQuad);
    }
    
    /// <summary>
    /// Stop the shake animation
    /// </summary>
    private void StopShakeAnimation()
    {
        if (shakeAnimation != null)
        {
            shakeAnimation.Kill();
            shakeAnimation = null;
        }
    }
}
