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
    public Toggle keepExistingPotionsToggle;
    public Button discardPotionButton;
    public TextMeshProUGUI roundProgressText;
    
    [Header("Game References")]
    public Basket2D basket;
    public GameObject animatedPotionPrefab;
    public Transform[] shelves = new Transform[3];
    public Transform cauldron;
    
    [Header("Animation Settings")]
    [SerializeField] private float ingredientAnimationDuration = 0.5f;
    [SerializeField] private float delayBetweenIngredients = 0.1f;
    [SerializeField] private Ease ingredientAnimationEase = Ease.InOutQuad;
    [SerializeField] private float potionAnimationDuration = 0.35f;
    [SerializeField] private Ease potionAnimationEase = Ease.InOutQuad;
    
    [Header("Animation Database")]
    public PotionAnimationDatabase animationDatabase;
    
    private List<IngredientPrefab> selectedIngredients = new List<IngredientPrefab>();
    private List<PotionRecipe> recipes = new List<PotionRecipe>();
    private PotionRecipe currentSelectedRecipe = null;
    private bool isAnimating = false; // Prevent multiple concoctions during animation
    private int totalLevelsCreated = 0;

    void Start()
    {
        concoctButton.gameObject.SetActive(false);
        concoctButton.onClick.AddListener(ConcoctPotion);
        
        discardPotionButton.gameObject.SetActive(false);
        discardPotionButton.onClick.AddListener(CancelReplacement);
        
        // Initialize recipe display text
        UpdateRecipeDisplay();
        
        CreateAllRecipes();
    }
    
    void CreateAllRecipes()
    {
        recipes.Clear();
        
        // Recipe priority: most specific (uses most ingredients) to most general
        // Format: CreateRecipe([magic ingredients], [natural ingredients])
        // [plants, animals, minerals] for each type
        
        // 2 IDENTICAL MAGIC INGREDIENTS - Most specific recipes
        CreateRecipe(new int[]{2, 0, 0}, new int[]{0, 0, 0}); // 2 magic plants
        CreateRecipe(new int[]{0, 2, 0}, new int[]{0, 0, 0}); // 2 magic animals
        CreateRecipe(new int[]{0, 0, 2}, new int[]{0, 0, 0}); // 2 magic minerals
        // 2 DIFFERENT MAGIC INGREDIENTS
        CreateRecipe(new int[]{1, 1, 0}, new int[]{0, 0, 0}); // magic plant + animal
        CreateRecipe(new int[]{1, 0, 1}, new int[]{0, 0, 0}); // magic plant + mineral
        CreateRecipe(new int[]{0, 1, 1}, new int[]{0, 0, 0}); // magic animal + mineral
        // 2 DIFFERENT INGREDIENTS - Magic Plant + Natural
        CreateRecipe(new int[]{1, 0, 0}, new int[]{1, 0, 0}); // magic plant + natural plant
        CreateRecipe(new int[]{1, 0, 0}, new int[]{0, 1, 0}); // magic plant + natural animal
        CreateRecipe(new int[]{1, 0, 0}, new int[]{0, 0, 1}); // magic plant + natural mineral
        // 2 DIFFERENT INGREDIENTS - Magic Animal + Natural
        CreateRecipe(new int[]{0, 1, 0}, new int[]{1, 0, 0}); // magic animal + natural plant
        CreateRecipe(new int[]{0, 1, 0}, new int[]{0, 1, 0}); // magic animal + natural animal
        CreateRecipe(new int[]{0, 1, 0}, new int[]{0, 0, 1}); // magic animal + natural mineral
        // 2 DIFFERENT INGREDIENTS - Magic Mineral + Natural
        CreateRecipe(new int[]{0, 0, 1}, new int[]{1, 0, 0}); // magic mineral + natural plant
        CreateRecipe(new int[]{0, 0, 1}, new int[]{0, 1, 0}); // magic mineral + natural animal
        CreateRecipe(new int[]{0, 0, 1}, new int[]{0, 0, 1}); // magic mineral + natural mineral

        // 1 INGREDIENT - General fallback recipes
        CreateRecipe(new int[]{1, 0, 0}, new int[]{0, 0, 0}); // 1 magic plant
        CreateRecipe(new int[]{0, 1, 0}, new int[]{0, 0, 0}); // 1 magic animal
        CreateRecipe(new int[]{0, 0, 1}, new int[]{0, 0, 0}); // 1 magic mineral
    }
    
    void CreateRecipe(string name, string description, int level, int duration, params CategoryRequirement[] requirements)
    {
        recipes.Add(new PotionRecipe
        {
            potionName = name,
            description = description,
            level = level,
            duration = duration,
            categoryRequirements = new List<CategoryRequirement>(requirements)
        });
    }
    
    /// <summary>
    /// New scalable recipe creation method
    /// </summary>
    void CreateRecipe(int[] magicIngredients, int[] naturalIngredients)
    {
        // magicIngredients = [plants, animals, minerals]
        // naturalIngredients = [plants, animals, minerals]
        
        List<CategoryRequirement> requirements = new List<CategoryRequirement>();
        
        // Add magic ingredient requirements
        if (magicIngredients[0] > 0) requirements.Add(RecipeUtils.MagicPlant(magicIngredients[0], magicIngredients[0]));
        if (magicIngredients[1] > 0) requirements.Add(RecipeUtils.MagicAnimal(magicIngredients[1], magicIngredients[1]));
        if (magicIngredients[2] > 0) requirements.Add(RecipeUtils.MagicMineral(magicIngredients[2], magicIngredients[2]));
        
        // Add natural ingredient requirements
        if (naturalIngredients[0] > 0) requirements.Add(RecipeUtils.NaturalPlant(naturalIngredients[0], naturalIngredients[0]));
        if (naturalIngredients[1] > 0) requirements.Add(RecipeUtils.NaturalAnimal(naturalIngredients[1], naturalIngredients[1]));
        if (naturalIngredients[2] > 0) requirements.Add(RecipeUtils.NaturalMineral(naturalIngredients[2], naturalIngredients[2]));
        
        // Calculate properties automatically
        int level = CalculatePotionLevel(magicIngredients, naturalIngredients);
        string name = GeneratePotionName(magicIngredients, naturalIngredients, level);
        string description = GeneratePotionDescription(magicIngredients, naturalIngredients);
        int duration = CalculatePotionDuration(magicIngredients, naturalIngredients);
        
        recipes.Add(new PotionRecipe
        {
            potionName = name,
            description = description,
            level = level,
            duration = duration,
            categoryRequirements = requirements
        });
    }
    
    string GeneratePotionName(int[] magic, int[] natural, int level)
    {
        int totalMagic = magic[0] + magic[1] + magic[2];
        int totalNatural = natural[0] + natural[1] + natural[2];
        
        string baseName;
        
        // Handle identical ingredients
        if (magic[0] == 2) baseName = "Double Magic Plant Potion";
        else if (magic[1] == 2) baseName = "Double Magic Animal Potion";
        else if (magic[2] == 2) baseName = "Double Magic Mineral Potion";
        
        // Handle mixed magic ingredients
        else if (totalMagic == 2 && totalNatural == 0)
        {
            if (magic[0] > 0 && magic[1] > 0) baseName = "Magic Plant & Animal Potion";
            else if (magic[0] > 0 && magic[2] > 0) baseName = "Magic Plant & Mineral Potion";
            else if (magic[1] > 0 && magic[2] > 0) baseName = "Magic Animal & Mineral Potion";
            else baseName = "Mixed Potion";
        }
        
        // Handle hybrid (magic + natural)
        else if (totalMagic == 1 && totalNatural == 1)
        {
            if (magic[0] > 0 && natural[0] > 0) baseName = "Hybrid Plant Potion";
            else if (magic[1] > 0 && natural[1] > 0) baseName = "Hybrid Animal Potion";
            else if (magic[2] > 0 && natural[2] > 0) baseName = "Hybrid Mineral Potion";
            else baseName = "Mixed Potion";
        }
        
        // Handle single ingredients
        else if (totalMagic == 1 && totalNatural == 0)
        {
            if (magic[0] > 0) baseName = "Magic Plant Potion";
            else if (magic[1] > 0) baseName = "Magic Animal Potion";
            else if (magic[2] > 0) baseName = "Magic Mineral Potion";
            else baseName = "Mixed Potion";
        }
        
        // Default case
        else
        {
            baseName = "Mixed Potion";
        }
        
        // Add level to the name
        return $"{baseName} Lv.{level}";
    }
    
    string GeneratePotionDescription(int[] magic, int[] natural)
    {
        int totalMagic = magic[0] + magic[1] + magic[2];
        int totalNatural = natural[0] + natural[1] + natural[2];
        
        if (magic[0] == 2) return "Enhanced plant magic";
        if (magic[1] == 2) return "Enhanced animal magic";
        if (magic[2] == 2) return "Very powerful mineral magic";
        
        if (totalMagic == 2 && totalNatural == 0)
        {
            if (magic[0] > 0 && magic[1] > 0) return "Mixed essence of plant and animal";
            if (magic[0] > 0 && magic[2] > 0) return "Mixed essence of plant and mineral";
            if (magic[1] > 0 && magic[2] > 0) return "Mixed essence of animal and mineral";
        }
        
        if (totalMagic == 1 && totalNatural == 1)
        {
            if (magic[0] > 0) return "Magic and natural plant fusion";
            if (magic[1] > 0) return "Magic and natural animal fusion";
            if (magic[2] > 0) return "Magic and natural mineral fusion";
        }
        
        if (totalMagic == 1 && totalNatural == 0)
        {
            if (magic[0] > 0) return "A basic potion made from one magic plant";
            if (magic[1] > 0) return "A basic potion made from one magic animal";
            if (magic[2] > 0) return "A powerful potion made from one magic mineral";
        }
        
        return "A mixed potion with various ingredients";
    }
    
    int CalculatePotionLevel(int[] magic, int[] natural)
    {
        // Each magic ingredient = 2 points, each natural ingredient = 1 point
        // Score per type: sum all points for each type
        int plantScore = magic[0] * 2 + natural[0] * 1;     // Plants
        int animalScore = magic[1] * 2 + natural[1] * 1;    // Animals  
        int mineralScore = magic[2] * 2 + natural[2] * 1;   // Minerals
        
        // Level = highest score among the 3 types
        return Mathf.Max(plantScore, animalScore, mineralScore);
    }
    
    int CalculatePotionDuration(int[] magic, int[] natural)
    {
        int baseDuration = 30;
        
        // Duration based on ingredient strength
        int magicMineral = magic[2];
        int totalIngredients = magic[0] + magic[1] + magic[2] + natural[0] + natural[1] + natural[2];
        
        // Minerals increase duration significantly
        baseDuration += magicMineral * 30;
        
        // Multiple ingredients increase duration
        if (totalIngredients >= 2) baseDuration += 15;
        
        return baseDuration;
    }
    
    public void AddSelectedIngredient(IngredientPrefab ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Add(ingredient);
            UpdateRecipeSelection();
            UpdateButtonVisibility();
        }
    }
    
    public void RemoveSelectedIngredient(IngredientPrefab ingredient)
    {
        selectedIngredients.Remove(ingredient);
        UpdateRecipeSelection();
        UpdateButtonVisibility();
    }
    
    void UpdateButtonVisibility()
    {
        // Show button only if we have ingredients and we're not currently animating
        concoctButton.gameObject.SetActive(selectedIngredients.Count > 0 && !isAnimating);
        // Also disable interactability during animation
        concoctButton.interactable = !isAnimating;
        // Disable the checkbox during animation to prevent changing behavior mid-process
        keepExistingPotionsToggle.interactable = !isAnimating;
    }
    
    void UpdateRecipeSelection()
    {
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
        
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        foreach (PotionRecipe recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients)) return recipe;
        }
        
        return null; // No matching recipe found
    }

    void ConcoctPotion()
    {
        if (selectedIngredients.Count == 0 || currentSelectedRecipe == null || isAnimating) return;
        
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
        // Disable ingredient interactions during potion creation
        SetIngredientInteractionsEnabled(false);
        // Store ingredients for replacement before starting animation
        List<IngredientPrefab> ingredientsToReplace = new List<IngredientPrefab>(selectedIngredients);
        // Sort ingredients by X position (leftmost first)
        List<IngredientPrefab> sortedIngredients = new List<IngredientPrefab>(selectedIngredients);
        sortedIngredients.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        // Animate each ingredient one by one
        foreach (var ingredient in sortedIngredients)
        {
            yield return StartCoroutine(AnimateIngredientToCauldron(ingredient));
            yield return new WaitForSeconds(delayBetweenIngredients);
        }
        // All ingredients animated, now create the potion and wait until it's completely finalized
        yield return StartCoroutine(CreateAndFinalizePotionCompletely(potionId, ingredientsToReplace));
        // Clean up after everything is completely done
        selectedIngredients.Clear();
        currentSelectedRecipe = null;
        UpdateRecipeDisplay();
        // finally re-enable everything - the potion creation cycle is 100% complete
        isAnimating = false;
        UpdateButtonVisibility();
        SetIngredientInteractionsEnabled(true);
        
        Debug.Log("Potion creation cycle finished - ready for next potion!");
    }
    
    /// <summary>
    /// Animate a single ingredient to the cauldron and destroy it
    /// </summary>
    IEnumerator AnimateIngredientToCauldron(IngredientPrefab ingredient)
    {
        if (ingredient == null) yield break;
        
        Vector3 startPosition = ingredient.transform.position;
        Vector3 targetPosition = cauldron.position;
        // Animate movement to cauldron
        Tween moveTween = ingredient.transform.DOMove(targetPosition, ingredientAnimationDuration)
            .SetEase(ingredientAnimationEase);
        // Add some scaling effect as it approaches
        Tween scaleTween = ingredient.transform.DOScale(Vector3.zero, ingredientAnimationDuration * 0.8f)
            .SetDelay(ingredientAnimationDuration * 0.2f)
            .SetEase(Ease.InBack);
        // Wait for animation to complete
        yield return moveTween.WaitForCompletion();
    }

    /// <summary>
    /// Create potion and handle ALL scenarios until completely finalized
    /// </summary>
    IEnumerator CreateAndFinalizePotionCompletely(string potionId, List<IngredientPrefab> ingredientsToReplace)
    {
        if (currentSelectedRecipe == null) yield break;
        // Find first available shelf
        Transform availableShelf = FindFirstAvailableShelf();
        if (availableShelf == null)
        {
            // SCENARIO 1: Shelves full + keep existing potions
            if (keepExistingPotionsToggle.isOn)
            {
                Debug.Log("All shelves are full! Keeping existing potions as requested. Replacing ingredients with new ones...");
                basket.ReplaceIngredients(ingredientsToReplace);
            }
            // SCENARIO 2: Shelves full + replacement mode
            else
            {
                Debug.Log("All shelves are full! Creating potion at cauldron and waiting for replacement choice...");
                CreatePotionAtCauldronForReplacement(potionId, ingredientsToReplace);
                // wait until player makes decision (replacement or cancellation)
                yield return StartCoroutine(WaitForReplacementDecision());
            }
        }
        else
        {
            // SCENARIO 3: Shelf available - normal flow
            yield return StartCoroutine(CreatePotionOnAvailableShelf(potionId, availableShelf, ingredientsToReplace));
        }
    }
    
    /// <summary>
    /// Create potion on available shelf (normal flow)
    /// </summary>
    IEnumerator CreatePotionOnAvailableShelf(string potionId, Transform shelf, List<IngredientPrefab> ingredientsToReplace)
    {
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        PotionAnimationMapping mapping = animationDatabase?.GetMappingForIngredients(ingredients);
        // Create potion as child of shelf but position it at cauldron
        GameObject newPotion = Instantiate(animatedPotionPrefab, shelf);
        newPotion.name = $"Potion_{potionId}";
        newPotion.transform.position = cauldron.position;
        // Setup potion data
        AnimatedPotion animatedPotionComponent = newPotion.GetComponent<AnimatedPotion>();
        animatedPotionComponent.SetupPotion(ingredients, currentSelectedRecipe, mapping);
        animatedPotionComponent.SetInteractionsEnabled(false);
        // Wait a moment for the potion to appear
        yield return new WaitForSeconds(0.1f);
        // Animate potion to shelf position
        Vector3 shelfLocalPosition = Vector3.zero;
        Tween potionMoveTween = newPotion.transform.DOLocalMove(shelfLocalPosition, potionAnimationDuration)
            .SetEase(potionAnimationEase);
        // Optional scaling effect
        Tween potionScaleTween = newPotion.transform.DOScale(Vector3.one * 1.2f, potionAnimationDuration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);
        // Wait for animation to complete
        yield return potionMoveTween.WaitForCompletion();
        // Re-enable potion interactions
        animatedPotionComponent.SetInteractionsEnabled(true);
        // Replace ingredients AFTER potion is successfully placed
        Debug.Log("Replacing ingredients with new ones");
        basket.ReplaceIngredients(ingredientsToReplace);

        Debug.Log($"Potion {potionId} successfully created and placed on shelf!");
        
        // Add potion levels to round progress
        AddPotionLevels(currentSelectedRecipe.level);
    }
    
    void RemoveUsedIngredients()
    {
        // Note: ingredients are replaced elsewhere when potion is finalized
        selectedIngredients.Clear();
        currentSelectedRecipe = null;
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
    
    Transform FindFirstAvailableShelf()
    {
        for (int i = 0; i < shelves.Length; i++)
        {
            if (shelves[i] != null && IsShelfEmpty(shelves[i])) return shelves[i];
        }
        return null; // All shelves are full or not assigned
    }
    
    bool IsShelfEmpty(Transform shelf)
    {
        // Check if shelf has any children that are potions (ignore sprite children)
        int potionCount = 0;
        for (int i = 0; i < shelf.childCount; i++)
        {
            if (shelf.GetChild(i).GetComponent<AnimatedPotion>() != null) potionCount++;
        }
        return potionCount == 0;
    }
    
    /// <summary>
    /// Create potion at cauldron for replacement mode - potion stays at cauldron
    /// </summary>
    void CreatePotionAtCauldronForReplacement(string potionId, List<IngredientPrefab> ingredientsToReplace)
    {
        List<Ingredient> ingredients = GetSelectedIngredientsAsIngredients();
        PotionAnimationMapping mapping = animationDatabase?.GetMappingForIngredients(ingredients);
        // Create potion at cauldron position (no parent initially)
        GameObject newPotion = Instantiate(animatedPotionPrefab);
        newPotion.name = $"Potion_{potionId}";
        newPotion.transform.position = cauldron.position;
        // Setup potion data
        AnimatedPotion animatedPotionComponent = newPotion.GetComponent<AnimatedPotion>();
        animatedPotionComponent.SetupPotion(ingredients, currentSelectedRecipe, mapping);
        // Disable interactions while potion is waiting at cauldron
        animatedPotionComponent.SetInteractionsEnabled(false);
        // Store the pending potion for replacement mode
        StorePendingPotionData(potionId, newPotion, ingredientsToReplace);
        // Enable replacement mode on all existing potions
        EnableReplacementModeOnPotions(true);
        // Show instruction UI
        ShowReplacementInstructions(true);
        
        Debug.Log($"Potion {potionId} created at cauldron, waiting for replacement choice...");
    }
    
    /// <summary>
    /// Wait for the player to make a replacement decision
    /// </summary>
    IEnumerator WaitForReplacementDecision()
    {
        Debug.Log("Waiting for player replacement decision...");
        
        // Wait until replacement mode is no longer active (decision has been made)
        while (pendingRecipe != null)
        {
            yield return null; // Wait one frame
        }
    }
    
    /// <summary>
    /// Store the pending potion data while waiting for replacement choice
    /// </summary>
    void StorePendingPotionData(string potionId, GameObject pendingPotionObject = null, List<IngredientPrefab> ingredientsToReplace = null)
    {
        pendingPotionId = potionId;
        pendingIngredients = GetSelectedIngredientsAsIngredients();
        pendingRecipe = currentSelectedRecipe;
        pendingMapping = animationDatabase?.GetMappingForIngredients(pendingIngredients);
        pendingPotionAtCauldron = pendingPotionObject;
        pendingIngredientsToReplace = ingredientsToReplace;
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
                    if (potion != null) potion.SetReplacementMode(enable);
                }
            }
        }
    }
    
    /// <summary>
    /// Show or hide replacement instructions and discard button
    /// </summary>
    void ShowReplacementInstructions(bool show)
    {
        discardPotionButton.gameObject.SetActive(show);
    }
    
    /// <summary>
    /// Called when a potion is clicked during replacement mode
    /// </summary>
    public void OnPotionSelectedForReplacement(AnimatedPotion potionToReplace)
    {
        if (pendingRecipe == null) return;
        
        Debug.Log($"Replacing potion: {potionToReplace.potionName} with new potion: {pendingRecipe.potionName}");
        
        ShowReplacementInstructions(false);
        // Get the shelf of the potion to replace
        Transform shelf = potionToReplace.transform.parent;
        // Start the replacement sequence
        StartCoroutine(ReplacePotionSequence(potionToReplace, shelf));
    }
    
    /// <summary>
    /// Handle the full replacement sequence with proper timing
    /// </summary>
    IEnumerator ReplacePotionSequence(AnimatedPotion potionToReplace, Transform shelf)
    {
        // Animate the destruction of the old potion
        yield return StartCoroutine(AnimatePotionDestruction(potionToReplace.gameObject, false));
        // Add a small pause between destruction and new potion arrival for better visual flow
        yield return new WaitForSeconds(0.3f);
        // Handle the new potion
        if (pendingPotionAtCauldron != null)
        {
            pendingPotionAtCauldron.transform.SetParent(shelf, true);
            // Animate to shelf center using world coordinates
            Vector3 shelfWorldPosition = shelf.position;
            Tween potionMoveTween = pendingPotionAtCauldron.transform.DOMove(shelfWorldPosition, potionAnimationDuration)
                .SetEase(potionAnimationEase);
            // Add some scaling effect during movement
            Tween potionScaleTween = pendingPotionAtCauldron.transform.DOScale(Vector3.one * 1.2f, potionAnimationDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo);
            yield return potionMoveTween.WaitForCompletion();
            // After animation, ensure correct local position
            pendingPotionAtCauldron.transform.localPosition = Vector3.zero;
            // Re-enable interactions now that potion is on shelf
            AnimatedPotion potionComponent = pendingPotionAtCauldron.GetComponent<AnimatedPotion>();
            potionComponent.SetInteractionsEnabled(true);
        }
        
        // Replace used ingredients with new ones after successful replacement but BEFORE clearing data
        if (pendingIngredientsToReplace != null)
        {
            Debug.Log("Replacing ingredients with new ones...");
            basket.ReplaceIngredients(pendingIngredientsToReplace);
        }
        
        // Add potion levels to round progress (before clearing pending data)
        if (pendingRecipe != null)
        {
            AddPotionLevels(pendingRecipe.level);
        }
        
        // End replacement mode after replacing ingredients (this will clean up data)
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
        ShowReplacementInstructions(false);
        // Animate destruction of the pending potion at cauldron if it exists
        if (pendingPotionAtCauldron != null) StartCoroutine(AnimatePotionDestruction(pendingPotionAtCauldron));
        else CompleteCancellation();
    }
    
    /// <summary>
    /// Animate potion destruction with simple scale effect
    /// </summary>
    IEnumerator AnimatePotionDestruction(GameObject potionToDestroy, bool shouldCompleteCancellation = true)
    {        
        // completely disable all colliders to prevent mouse events
        Collider2D[] allColliders = potionToDestroy.GetComponents<Collider2D>();
        foreach (var collider in allColliders) collider.enabled = false;
        // Kill any existing DOTween animations on this object
        potionToDestroy.transform.DOKill();
        // Use DOTween again now that we fixed the OnMouseExit interference
        yield return potionToDestroy.transform.DOScale(Vector3.zero, potionAnimationDuration)
            .SetEase(Ease.InBack)
            .WaitForCompletion();
        DestroyImmediate(potionToDestroy);
        // Complete cancellation only if this is for cancellation (not replacement)
        if (shouldCompleteCancellation) CompleteCancellation();
    }
    
    /// <summary>
    /// Complete the cancellation process after animation
    /// </summary>
    void CompleteCancellation()
    {
        // Replace used ingredients since no potion was placed
        if (pendingIngredientsToReplace != null)
        {
            Debug.Log("Replacing ingredients with new ones...");
            basket.ReplaceIngredients(pendingIngredientsToReplace);
        }
        
        EndReplacementMode();
    }
    
    /// <summary>
    /// End replacement mode and clean up
    /// </summary>
    void EndReplacementMode()
    {
        EnableReplacementModeOnPotions(false);
        ClearPendingPotionData();
    }
    
    /// <summary>
    /// Clear stored pending potion data
    /// </summary>
    void ClearPendingPotionData()
    {
        Debug.Log("DEBUG: Clearing pending potion data");
        pendingPotionId = null;
        pendingIngredients = null;
        pendingRecipe = null;
        pendingMapping = null;
        pendingPotionAtCauldron = null;
        pendingIngredientsToReplace = null;
    }
    
    // Pending potion data for replacement mode
    private string pendingPotionId;
    private List<Ingredient> pendingIngredients;
    private PotionRecipe pendingRecipe;
    private PotionAnimationMapping pendingMapping;
    private GameObject pendingPotionAtCauldron; // The actual potion GameObject waiting at cauldron
    private List<IngredientPrefab> pendingIngredientsToReplace; // Ingredients to replace when potion is finalized
    
    /// <summary>
    /// Enable or disable ingredient interactions (colliders)
    /// </summary>
    void SetIngredientInteractionsEnabled(bool enabled)
    {
        // Find all ingredient prefabs in the scene
        IngredientPrefab[] allIngredients = FindObjectsOfType<IngredientPrefab>();
        foreach (IngredientPrefab ingredient in allIngredients)
        {
            if (ingredient.interactionCollider != null) ingredient.interactionCollider.enabled = enabled;
        }
        
        Debug.Log($"Ingredient interactions {(enabled ? "ENABLED" : "DISABLED")}");
    }
    
    /// <summary>
    /// Add levels when a potion is created
    /// </summary>
    void AddPotionLevels(int levels)
    {
        totalLevelsCreated += levels;
        roundProgressText.text = $"{totalLevelsCreated}";
    }
}
