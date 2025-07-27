using UnityEngine;

public enum IngredientFamily { Natural, Magic }
public enum IngredientSubFamily { Plant, Animal, Mineral }

[CreateAssetMenu(fileName = "NewIngredient", menuName = "PotionGame/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public IngredientFamily family;
    public IngredientSubFamily subFamily;
    public string description;
    public Sprite icon;
}
