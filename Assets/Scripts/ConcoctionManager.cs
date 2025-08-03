using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;

public class ConcoctionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button concoctButton;
    public TextMeshProUGUI recipeNameText;
    public Toggle keepExistingPotionsToggle; // Checkbox to keep existing potions
    
    [Header("Game References")]
    public Basket2D basket;
    public GameObject animatedPotionPrefab;
    public Transform[] shelves = new Transform[3]; // Array of 3 shelf transforms
    public Transform cauldron; // Reference to the cauldron transform
    
    [Header("Animation Settings")]
    [SerializeField] private float ingredientAnimationDuration = 1.0f;
    [SerializeField] private float delayBetweenIngredients = 0.3f;
    [SerializeField] private Ease ingredientAnimationEase = Ease.InOutQuad;
    
    [Header("Animation Database")]
    public PotionAnimationDatabase animationDatabase;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    private PotionRecipe currentSelectedRecipe = null; // Currently selected recipe
    private bool isAnimating = false; // Prevent multiple concoctions during animation

    void Start()
    {
        concoctButton.gameObject.SetActive(false);
        concoctButton.onClick.AddListener(ConcoctPotion);
        
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
        if (concoctButton != null) 
        {
            // Show button only if we have ingredients and we're not currently animating
            concoctButton.gameObject.SetActive(selectedIngredients.Count > 0 && !isAnimating);
            // Also disable interactability during animation
            concoctButton.interactable = !isAnimating;
        }
    }
    
    void UpdateRecipeSelection()
    {
        // Find the first recipe (most specific) that matches current ingredients
        currentSelectedRecipe = FindFirstMatchingRecipe();
        UpdateRecipeDisplay();
    }
    
    void UpdateRecipeDisplay()
    {
        if (currentSelectedRecipe != null) recipeNameText.text = currentSelectedRecipe.potionName;
        else recipeNameText.text = selectedIngredients.Count > 0 ? "No valid recipe" : "Select ingredients";
    }
    
    PotionRecipe FindFirstMatchingRecipe()
    {
        if (selectedIngredients.Count == 0) return null;
        
        // Convert selected ingredients to ingredient list
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        
        // Return the FIRST recipe that matches (most specific due to ordering)
        foreach (PotionRecipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients)) return recipe;
        }
        
        return null; // No matching recipe found
    }

    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0 || currentSelectedRecipe == null || isAnimating) return;
        
        // Generate unique potion ID based on ingredients used
        string potionId = PotionUtils.GeneratePotionId(GetSelectedIngredientsAsIngredients());
        
        Debug.Log($"Starting concoction of potion ID: {potionId}");
        
        // Start animation sequence
        StartCoroutine(AnimateIngredientsToCauldron(potionId));
    }
    
    /// <summary>
    /// Animate ingredients to cauldron one by one, then create potion
    /// </summary>
    IEnumerator AnimateIngredientsToCauldron(string potionId)
    {
        isAnimating = true;
        UpdateButtonVisibility();
        
        // Store ingredients for replacement BEFORE starting animation
        List<IngredientPrefab> ingredientsToReplace = new List<IngredientPrefab>(selectedIngredients);
        
        // Sort ingredients by X position (leftmost first)
        List<IngredientPrefab> sortedIngredients = new List<IngredientPrefab>(selectedIngredients);
        sortedIngredients.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        
        Debug.Log($"Animating {sortedIngredients.Count} ingredients to cauldron...");
        
        // Animate each ingredient one by one
        foreach (var ingredient in sortedIngredients)
        {
            yield return StartCoroutine(AnimateIngredientToCauldron(ingredient));
            yield return new WaitForSeconds(delayBetweenIngredients);
        }
        
        // All ingredients animated, now create the potion
        Debug.Log($"All ingredients consumed! Creating potion {potionId}");
        CreatePotionOnShelf(potionId);
        
        // Replace used ingredients with new ones from basket AFTER animation
        Debug.Log("Replacing ingredients with new ones...");
        basket.ReplaceIngredients(ingredientsToReplace);
        
        // Clean up
        selectedIngredients.Clear();
        currentSelectedRecipe = null;
        UpdateRecipeDisplay();
        isAnimating = false;
        UpdateButtonVisibility();
    }
    
    /// <summary>
    /// Animate a single ingredient to the cauldron and destroy it
    /// </summary>
    IEnumerator AnimateIngredientToCauldron(IngredientPrefab ingredient)
    {
        if (ingredient == null || cauldron == null) yield break;
        
        Vector3 startPosition = ingredient.transform.position;
        Vector3 targetPosition = cauldron.position;
        
        Debug.Log($"Animating {ingredient.name} from {startPosition} to {targetPosition}");
        
        // Animate movement to cauldron
        Tween moveTween = ingredient.transform.DOMove(targetPosition, ingredientAnimationDuration)
            .SetEase(ingredientAnimationEase);
            
        // Optional: Add some scaling effect as it approaches
        Tween scaleTween = ingredient.transform.DOScale(Vector3.zero, ingredientAnimationDuration * 0.8f)
            .SetDelay(ingredientAnimationDuration * 0.2f)
            .SetEase(Ease.InBack);
        
        // Wait for animation to complete
        yield return moveTween.WaitForCompletion();
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
            // Check if user wants to keep existing potions
            if (keepExistingPotionsToggle.isOn)
            {
                Debug.Log("All shelves are full! Keeping existing potions as requested.");
                ShowShelvesFullMessage();
                return;
            }

            Debug.Log("All shelves are full! Proposing potion replacement...");
            StartPotionReplacementProcess(potionId);
            return;
        }
        
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        PotionAnimationMapping mapping = animationDatabase?.GetMappingForIngredients(ingredients);
        
        GameObject newPotion = Instantiate(animatedPotionPrefab, availableShelf);
        newPotion.name = $"Potion_{potionId}";
        
        AnimatedPotion animatedPotionComponent = newPotion.GetComponent<AnimatedPotion>();
        animatedPotionComponent.SetupPotion(ingredients, currentSelectedRecipe, mapping);
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
    
    /// <summary>
    /// Show message when shelves are full and keeping existing potions
    /// </summary>
    void ShowShelvesFullMessage()
    {
        Debug.Log("Cannot create new potion: All shelves are full and 'Keep existing potions' is enabled.");
        // TODO: You can add a UI popup here later
        // For now, don't consume ingredients since no potion was created
    }
    
    /// <summary>
    /// Start the potion replacement process when shelves are full
    /// </summary>
    void StartPotionReplacementProcess(string newPotionId)
    {
        Debug.Log("=== POTION REPLACEMENT MODE ===");
        Debug.Log("All shelves are full! Click on a potion to replace it, or press ESC to cancel.");
        
        // Store the new potion data for later use
        StorePendingPotionData(newPotionId);
        
        // Enable replacement mode on all existing potions
        EnableReplacementModeOnPotions(true);
        
        // Show instruction UI (you can add a UI element later)
        ShowReplacementInstructions(true);
    }
    
    /// <summary>
    /// Store the pending potion data while waiting for replacement choice
    /// </summary>
    void StorePendingPotionData(string potionId)
    {
        pendingPotionId = potionId;
        pendingIngredients = GetSelectedIngredientsAsIngredients();
        pendingRecipe = currentSelectedRecipe;
        pendingMapping = animationDatabase?.GetMappingForIngredients(pendingIngredients);
    }
    
    /// <summary>
    /// Enable or disable replacement mode on all potions
    /// </summary>
    void EnableReplacementModeOnPotions(bool enable)
    {
        foreach (Transform shelf in shelves)
        {
            if (shelf != null)
            {
                for (int i = 0; i < shelf.childCount; i++)
                {
                    AnimatedPotion potion = shelf.GetChild(i).GetComponent<AnimatedPotion>();
                    if (potion != null)
                    {
                        potion.SetReplacementMode(enable);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Show or hide replacement instructions
    /// </summary>
    void ShowReplacementInstructions(bool show)
    {
        // For now just debug log, you can add UI later
        if (show)
        {
            Debug.Log("INSTRUCTIONS: Click on any potion to replace it with the new one, or press ESC to keep all potions.");
        }
    }
    
    /// <summary>
    /// Called when a potion is clicked during replacement mode
    /// </summary>
    public void OnPotionSelectedForReplacement(AnimatedPotion potionToReplace)
    {
        if (pendingRecipe == null) return;
        
        Debug.Log($"Replacing potion: {potionToReplace.potionName} with new potion: {pendingRecipe.potionName}");
        
        // Get the shelf of the potion to replace
        Transform shelf = potionToReplace.transform.parent;
        
        // Destroy the old potion
        DestroyImmediate(potionToReplace.gameObject);
        
        // Create the new potion on the same shelf
        GameObject newPotion = Instantiate(animatedPotionPrefab, shelf);
        newPotion.name = $"Potion_{pendingPotionId}";
        
        AnimatedPotion animatedPotionComponent = newPotion.GetComponent<AnimatedPotion>();
        if (animatedPotionComponent != null)
        {
            animatedPotionComponent.SetupPotion(pendingIngredients, pendingRecipe, pendingMapping);
        }
        
        // End replacement mode
        EndReplacementMode();
        
        // Remove used ingredients
        RemoveUsedIngredients();
    }
    
    /// <summary>
    /// Cancel the replacement process
    /// </summary>
    public void CancelReplacement()
    {
        Debug.Log("Potion replacement cancelled. Keeping all existing potions.");
        EndReplacementMode();
        
        // Don't remove ingredients since the potion wasn't created
    }
    
    /// <summary>
    /// End replacement mode and clean up
    /// </summary>
    void EndReplacementMode()
    {
        EnableReplacementModeOnPotions(false);
        ShowReplacementInstructions(false);
        ClearPendingPotionData();
    }
    
    /// <summary>
    /// Clear stored pending potion data
    /// </summary>
    void ClearPendingPotionData()
    {
        pendingPotionId = null;
        pendingIngredients = null;
        pendingRecipe = null;
        pendingMapping = null;
    }
    
    /// <summary>
    /// Handle ESC key to cancel replacement
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pendingRecipe != null)
        {
            CancelReplacement();
        }
    }
    
    // Pending potion data for replacement mode
    private string pendingPotionId;
    private List<Ingredient> pendingIngredients;
    private PotionRecipe pendingRecipe;
    private PotionAnimationMapping pendingMapping;
}
