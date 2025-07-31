using UnityEngine;
using DG.Tweening;

public class IngredientPrefab : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer iconRenderer;
    public BoxCollider2D interactionCollider;
    
    [Header("Selection Settings")]
    public float selectionOffset = 0.5f;
    public float animationDuration = 0.3f;
    public Ease animationEase = Ease.OutBack;
    
    private bool isSelected = false;
    private Vector3 originalPosition;
    private Ingredient ingredient;
    private ConcoctionManager concoctionManager;
    
    void Start()
    {
        originalPosition = transform.position;
        concoctionManager = FindObjectOfType<ConcoctionManager>();
        
        Vector2 spriteSize = iconRenderer.sprite.bounds.size;
        interactionCollider.size = spriteSize;
    }
    
    public void SetIngredient(Ingredient ing)
    {
        ingredient = ing;
        iconRenderer.sprite = ing.icon;
        Vector2 spriteSize = ing.icon.bounds.size;
        interactionCollider.size = spriteSize;
    }
    
    public Ingredient GetIngredient()
    {
        return ingredient;
    }
    
    void OnMouseDown()
    {
        ToggleSelection();
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
        // Arrêter toute animation en cours pour éviter les conflits
        transform.DOKill();
        
        Vector3 targetPosition = isSelected ? 
            originalPosition + new Vector3(0, selectionOffset, 0) : 
            originalPosition;
            
        // Animation fluide vers la position cible
        transform.DOMove(targetPosition, animationDuration)
            .SetEase(animationEase);
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
    
    void OnDestroy()
    {
        // Nettoyer toutes les animations DOTween de cet objet
        transform.DOKill();
    }
}
