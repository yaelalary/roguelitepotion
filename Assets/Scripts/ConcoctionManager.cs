using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    
    [Header("Game References")]
    public Basket2D basket;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    
    void Start()
    {
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
        if (concoctButton != null) concoctButton.gameObject.SetActive(selectedIngredients.Count > 0);
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

        // TODO: create potion logic here
        
        RemoveUsedIngredients();
    }
    
    void RemoveUsedIngredients()
    {
        if (basket != null) basket.ReplaceIngredients(selectedIngredients);
        else
        {
            foreach (var ingredientPrefab in selectedIngredients) Destroy(ingredientPrefab.gameObject);
        }
        selectedIngredients.Clear();
        UpdateButtonVisibility();
    }
    
    public List<IngredientPrefab> GetSelectedIngredients()
    {
        return new List<IngredientPrefab>(selectedIngredients);
    }
}
