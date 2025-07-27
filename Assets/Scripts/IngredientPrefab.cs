using UnityEngine;

public class IngredientPrefab : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer iconRenderer;
    public BoxCollider2D interactionCollider;
    
    [Header("Selection Settings")]
    public float selectionOffset = 0.5f;
    
    private bool isSelected = false;
    private Vector3 originalPosition;
    private Ingredient ingredient;
    private ConcoctionManager concoctionManager;
    
    void Start()
    {
        originalPosition = transform.position;
        concoctionManager = FindObjectOfType<ConcoctionManager>();
        
        if (interactionCollider != null && iconRenderer != null && iconRenderer.sprite != null)
        {
            Vector2 spriteSize = iconRenderer.sprite.bounds.size;
            interactionCollider.size = spriteSize;
        }
    }
    
    public void SetIngredient(Ingredient ing)
    {
        ingredient = ing;
        
        if (iconRenderer != null && ing.icon != null)
        {
            iconRenderer.sprite = ing.icon;
            if (interactionCollider != null)
            {
                Vector2 spriteSize = ing.icon.bounds.size;
                interactionCollider.size = spriteSize;
            }
        }
    }
    
    public Ingredient GetIngredient()
    {
        return ingredient;
    }
    
    void OnMouseDown()
    {
        if (ingredient != null)
        {
            Debug.Log("Clicked on ingredient: " + ingredient.ingredientName);
            ToggleSelection();
        }
    }
    
    public void ToggleSelection()
    {
        isSelected = !isSelected;
        UpdateVisuals();
        
        if (concoctionManager != null)
        {
            if (isSelected) concoctionManager.AddSelectedIngredient(this);
            else concoctionManager.RemoveSelectedIngredient(this);
        }
    }
    
    void UpdateVisuals()
    {
        if (isSelected) transform.position = originalPosition + new Vector3(0, selectionOffset, 0);
        else transform.position = originalPosition;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals();
    }
    
    public void UpdateOriginalPosition(Vector3 newOriginalPosition)
    {
        originalPosition = newOriginalPosition;
        if (!isSelected) transform.position = originalPosition;
    }
}
