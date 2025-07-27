using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    
    void Start()
    {
        // S'assurer que le bouton est caché au début
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(false);
            concoctButton.onClick.AddListener(ConcoctPotion);
        }
    }
    
    public void AddSelectedIngredient(IngredientPrefab ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateButtonVisibility();
        }
    }
    
    public void RemoveSelectedIngredient(IngredientPrefab ingredient)
    {
        selectedIngredients.Remove(ingredient);
        UpdateButtonVisibility();
    }
    
    void UpdateButtonVisibility()
    {
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(selectedIngredients.Count > 0);
        }
    }
    
    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0) return;
        
        Debug.Log($"Concoction d'une potion avec {selectedIngredients.Count} ingrédients:");
        
        foreach (var ingredientPrefab in selectedIngredients)
        {
            var ingredient = ingredientPrefab.GetIngredient();
            Debug.Log($"- {ingredient.ingredientName} ({ingredient.family}, {ingredient.subFamily})");
        }
        
        // Ici vous pourrez ajouter la logique de création de potion
        // Calculer les effets, le score, etc.
        
        // Optionnel : supprimer les ingrédients utilisés
        // RemoveUsedIngredients();
    }
    
    void RemoveUsedIngredients()
    {
        foreach (var ingredientPrefab in selectedIngredients)
        {
            Destroy(ingredientPrefab.gameObject);
        }
        selectedIngredients.Clear();
        UpdateButtonVisibility();
    }
    
    public List<IngredientPrefab> GetSelectedIngredients()
    {
        return new List<IngredientPrefab>(selectedIngredients);
    }
}
