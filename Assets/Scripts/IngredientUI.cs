using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;

    private bool isSelected = false;
    private Vector2 originalPosition;

    public void SetIngredient(Ingredient ingredient)
    {
        iconImage.sprite = ingredient.icon;
        nameText.text = ingredient.ingredientName;
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked on ingredient: " + nameText.text);
        ToggleSelection();
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        var rect = GetComponent<RectTransform>();
        if (isSelected)
        {
            rect.anchoredPosition = originalPosition + new Vector2(0, 30f); // Monte de 30 pixels
        }
        else
        {
            rect.anchoredPosition = originalPosition;
        }
    }
}