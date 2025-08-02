using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    public TextMeshProUGUI recipeNameText;
    
    [Header("Game References")]
    public Basket2D basket;
    public GameObject animatedPotionPrefab;
    public Transform[] shelves = new Transform[3]; // Array of 3 shelf transforms
    
    [Header("Animation Database")]
    public PotionAnimationDatabase animationDatabase;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    private PotionRecipe currentSelectedRecipe = null; // Currently selected recipe

    void Start()
    {
        if (concoctButton != null)
        {
            concoctButton.gameObject.SetActive(false);
            concoctButton.onClick.AddListener(ConcoctPotion);
        }
        
        // Initialize recipe display text
        UpdateRecipeDisplay();
        
        CreateAllRecipes();
    }
    
    void CreateAllRecipes()
    {
        recipes.Clear();
        
        // Recipe priority: most specific (uses most ingredients) to most general
        
        // 2 IDENTICAL INGREDIENTS - Most specific recipes
        CreateRecipe("Double Magic Plant Potion", "Enhanced plant magic", 3, 1, 60,
                    RecipeUtils.MagicPlant(2, 2));
        
        CreateRecipe("Double Magic Animal Potion", "Enhanced animal magic", 3, 1, 60,
                    RecipeUtils.MagicAnimal(2, 2));
        
        CreateRecipe("Double Magic Mineral Potion", "Very powerful mineral magic", 3, 1, 90,
                    RecipeUtils.MagicMineral(2, 2));
        
        // 2 DIFFERENT MAGIC INGREDIENTS
        CreateRecipe("Magic Plant & Animal Potion", "Mixed essence of plant and animal", 2, 2, 45,
                    RecipeUtils.MagicPlant(1, 1),
                    RecipeUtils.MagicAnimal(1, 1));
        
        CreateRecipe("Magic Plant & Mineral Potion", "Mixed essence of plant and mineral", 2, 2, 45,
                    RecipeUtils.MagicPlant(1, 1),
                    RecipeUtils.MagicMineral(1, 1));
        
        CreateRecipe("Magic Animal & Mineral Potion", "Mixed essence of animal and mineral", 2, 2, 45,
                    RecipeUtils.MagicAnimal(1, 1),
                    RecipeUtils.MagicMineral(1, 1));
        
        // 2 DIFFERENT INGREDIENTS - Magic + Natural
        CreateRecipe("Hybrid Plant Potion", "Magic and natural plant fusion", 2, 1, 30,
                    RecipeUtils.MagicPlant(1, 1),
                    RecipeUtils.NaturalPlant(1, 1));
        
        CreateRecipe("Hybrid Animal Potion", "Magic and natural animal fusion", 2, 1, 30,
                    RecipeUtils.MagicAnimal(1, 1),
                    RecipeUtils.NaturalAnimal(1, 1));
        
        CreateRecipe("Hybrid Mineral Potion", "Magic and natural mineral fusion", 2, 1, 30,
                    RecipeUtils.MagicMineral(1, 1),
                    RecipeUtils.NaturalMineral(1, 1));
        
        // 1 INGREDIENT - General fallback recipes
        CreateRecipe("Magic Plant Potion", "A basic potion made from one magic plant", 1, 1, 30,
                    RecipeUtils.MagicPlant(1, 1));
        
        CreateRecipe("Magic Animal Potion", "A basic potion made from one magic animal", 1, 1, 30,
                    RecipeUtils.MagicAnimal(1, 1));
        
        CreateRecipe("Magic Mineral Potion", "A powerful potion made from one magic mineral", 1, 2, 60,
                    RecipeUtils.MagicMineral(1, 1));
    }
    
    void CreateRecipe(string name, string description, int level, int subLevel, int duration, params CategoryRequirement[] requirements)
    {
        recipes.Add(new PotionRecipe
        {
            potionName = name,
            description = description,
            level = level,
            subLevel = subLevel,
            duration = duration,
            categoryRequirements = new List<CategoryRequirement>(requirements)
        });
    }
    
    public void AddSelectedIngredient(IngredientPrefab ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateRecipeSelection(); // Update selected recipe
            UpdateButtonVisibility();
        }
    }
    
    public void RemoveSelectedIngredient(IngredientPrefab ingredient)
    {
        selectedIngredients.Remove(ingredient);
        UpdateRecipeSelection(); // Update selected recipe
        UpdateButtonVisibility();
    }
    
    void UpdateButtonVisibility()
    {
        if (concoctButton != null) concoctButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }
    
    void UpdateRecipeSelection()
    {
        // Find the first recipe (most specific) that matches current ingredients
        currentSelectedRecipe = FindFirstMatchingRecipe();
        UpdateRecipeDisplay();
    }
    
    void UpdateRecipeDisplay()
    {
        if (recipeNameText != null)
        {
            if (currentSelectedRecipe != null)
            {
                recipeNameText.text = currentSelectedRecipe.potionName;
            }
            else
            {
                recipeNameText.text = selectedIngredients.Count > 0 ? "No valid recipe" : "Select ingredients";
            }
        }
    }
    
    PotionRecipe FindFirstMatchingRecipe()
    {
        if (selectedIngredients.Count == 0) return null;
        
        // Convert selected ingredients to ingredient list
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        
        // Return the FIRST recipe that matches (most specific due to ordering)
        foreach (PotionRecipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                return recipe;
            }
        }
        
        return null; // No matching recipe found
    }

    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0 || currentSelectedRecipe == null) return;
        
        // Generate unique potion ID based on ingredients used
        string potionId = PotionUtils.GeneratePotionId(GetSelectedIngredientsAsIngredients());
        
        Debug.Log("=== CONCOCTING POTION ===");
        Debug.Log($"Potion ID: {potionId}");
       
        CreatePotionOnShelf(potionId);
        
        // Remove used ingredients and hide button
        RemoveUsedIngredients();
    }
    
    void RemoveUsedIngredients()
    {
        basket.ReplaceIngredients(selectedIngredients);
        selectedIngredients.Clear();
        currentSelectedRecipe = null; // Reset selected recipe
        UpdateRecipeDisplay();
        UpdateButtonVisibility();
    }
    
    List<Ingredient> GetSelectedIngredientsAsIngredients()
    {
        List<Ingredient> ingredients = new List<Ingredient>();
        foreach (var ingredientPrefab in selectedIngredients)
        {
            ingredients.Add(ingredientPrefab.GetIngredient());
        }
        return ingredients;
    }

    void CreatePotionOnShelf(string potionId)
    {
        if (currentSelectedRecipe == null) return;
        
        // Find first available shelf
        Transform availableShelf = FindFirstAvailableShelf();
        if (availableShelf == null)
        {
            Debug.Log("All shelves are full! Cannot create potion.");
            return;
        }
        
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        PotionAnimationMapping mapping = animationDatabase?.GetMappingForIngredients(ingredients);
        
        GameObject newPotion = Instantiate(animatedPotionPrefab, availableShelf);
        newPotion.name = $"Potion_{potionId}";
        
        AnimatedPotion animatedPotionComponent = newPotion.GetComponent<AnimatedPotion>();
        if (animatedPotionComponent != null)
        {
            animatedPotionComponent.SetupPotion(ingredients, currentSelectedRecipe, mapping);
        }
    }
    
    Transform FindFirstAvailableShelf()
    {
        for (int i = 0; i < shelves.Length; i++)
        {
            if (shelves[i] != null && IsShelfEmpty(shelves[i]))
            {
                return shelves[i];
            }
        }
        return null; // All shelves are full or not assigned
    }
    
    bool IsShelfEmpty(Transform shelf)
    {
        // Check if shelf has any children that are potions (ignore sprite children)
        int potionCount = 0;
        for (int i = 0; i < shelf.childCount; i++)
        {
            if (shelf.GetChild(i).GetComponent<AnimatedPotion>() != null)
            {
                potionCount++;
            }
        }
        return potionCount == 0;
    }
}
