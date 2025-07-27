using UnityEngine;
using System.Collections.Generic;

public class Basket2D : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform ingredientsContainer;
    public GameObject ingredientPrefab;
    
    [Header("Ingredients Display")]
    public int ingredientsToDraw = 5;
    public float ingredientSpacing = 1.5f;
    
    private List<GameObject> drawnIngredients = new List<GameObject>();
    
    void Start()
    {
        DrawIngredients();
    }
    
    void DrawIngredients()
    {
        for (int i = 0; i < ingredientsToDraw; i++)
        {
            Ingredient ingredient = deckManager.DrawIngredient();
            if (ingredient == null) break;
            CreateIngredient(ingredient, i);
        }
    }
    
    void CreateIngredient(Ingredient ingredient, int index)
    {
        GameObject ingredientObj = Instantiate(ingredientPrefab);
        ingredientObj.name = $"Ingredient_{ingredient.ingredientName}";
        
        float totalWidth = (ingredientsToDraw - 1) * ingredientSpacing;
        float startX = -totalWidth / 2f;
        Vector3 ingredientPosition = ingredientsContainer.position + new Vector3(startX + index * ingredientSpacing, 0, 0);
        ingredientObj.transform.position = ingredientPosition;
        
        IngredientPrefab ingredientScript = ingredientObj.GetComponent<IngredientPrefab>();
        if (ingredientScript != null) ingredientScript.SetIngredient(ingredient);
        drawnIngredients.Add(ingredientObj);
    }
    
    public void ReplaceIngredients(List<IngredientPrefab> usedIngredients)
    {
        List<int> usedPositions = new List<int>();
        
        foreach (var usedIngredient in usedIngredients)
        {
            int index = -1;
            for (int i = 0; i < drawnIngredients.Count; i++)
            {
                if (drawnIngredients[i] != null)
                {
                    IngredientPrefab prefabScript = drawnIngredients[i].GetComponent<IngredientPrefab>();
                    if (prefabScript == usedIngredient)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                usedPositions.Add(index);
                Destroy(drawnIngredients[index]);
                drawnIngredients[index] = null;
            }
        }
        
        foreach (int position in usedPositions)
        {
            Ingredient newIngredient = deckManager.DrawIngredient();
            if (newIngredient != null)
            {
                GameObject newIngredientObj = Instantiate(ingredientPrefab);
                newIngredientObj.name = $"Ingredient_{newIngredient.ingredientName}";
                
                IngredientPrefab ingredientScript = newIngredientObj.GetComponent<IngredientPrefab>();
                if (ingredientScript != null) ingredientScript.SetIngredient(newIngredient);
                
                drawnIngredients[position] = newIngredientObj;
            }
            else
            {
                drawnIngredients[position] = null;
            }
        }
        
        RepositionIngredients();
    }
    
    void RepositionIngredients()
    {
        List<int> activeIndices = new List<int>();
        for (int i = 0; i < drawnIngredients.Count; i++)
        {
            if (drawnIngredients[i] != null) activeIndices.Add(i);
        }
        
        int activeCount = activeIndices.Count;
        if (activeCount == 0) return;
        
        float totalWidth = (activeCount - 1) * ingredientSpacing;
        float startX = -totalWidth / 2f;
        for (int i = 0; i < activeCount; i++)
        {
            int originalIndex = activeIndices[i];
            Vector3 newPosition = ingredientsContainer.position + new Vector3(startX + i * ingredientSpacing, 0, 0);
            drawnIngredients[originalIndex].transform.position = newPosition;
            
            IngredientPrefab ingredientScript = drawnIngredients[originalIndex].GetComponent<IngredientPrefab>();
            if (ingredientScript != null) ingredientScript.UpdateOriginalPosition(newPosition);
        }
    }
}
