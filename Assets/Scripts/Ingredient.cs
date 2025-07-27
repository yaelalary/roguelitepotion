using UnityEngine;

public enum IngredientFamily { Natural, Magic }
public enum IngredientSubFamily { Plant, Animal, Mineral }

[CreateAssetMenu(fileName = "NewIngredient", menuName = "PotionGame/Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField]
    private string ingredientName;
    
    public IngredientFamily family;
    public IngredientSubFamily subFamily;
    public string description;
    public Sprite icon;
    
    // Property to access the ingredient name
    public string IngredientName => ingredientName;
    
    // Method to set ingredient name (validates against constants)
    public void SetIngredientName(string name)
    {
        if (IngredientNames.IsValidIngredientName(name))
        {
            ingredientName = name;
        }
        else
        {
            Debug.LogWarning($"Invalid ingredient name: {name}. Using default value.");
            ingredientName = IngredientNames.FLOWER; // Default fallback
        }
    }
    
    void OnValidate()
    {
        // Auto-fix invalid ingredient names from display names to constants
        if (!string.IsNullOrEmpty(ingredientName))
        {
            // Check if it's a display name that needs conversion
            string[] allNames = IngredientNames.GetAllIngredientNames();
            bool foundMatch = false;
            
            foreach (string constantName in allNames)
            {
                string displayName = IngredientNames.GetDisplayName(constantName);
                if (ingredientName == displayName)
                {
                    // Convert display name to constant name
                    ingredientName = constantName;
                    foundMatch = true;
                    Debug.Log($"Auto-converted '{displayName}' to '{constantName}' in {name}");
                    break;
                }
                else if (ingredientName == constantName)
                {
                    foundMatch = true;
                    break;
                }
            }
            
            if (!foundMatch)
            {
                Debug.LogWarning($"Invalid ingredient name: {ingredientName} in {name}. Setting to default.");
                ingredientName = IngredientNames.FLOWER;
            }
        }
    }
}
