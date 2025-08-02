using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Utility class to create tooltip UI structure programmatically
/// This helps avoid having to manually create the UI in the scene
/// </summary>
public static class TooltipUIBuilder
{
    /// <summary>
    /// Create a complete tooltip UI structure under the given parent
    /// </summary>
    public static void CreateTooltipUI(Transform parent, out Canvas canvas, out GameObject panel, 
        out TextMeshProUGUI nameText, out TextMeshProUGUI levelText, 
        out TextMeshProUGUI descriptionText, out TextMeshProUGUI durationText, 
        out Image backgroundImage)
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("TooltipCanvas");
        canvasGO.transform.SetParent(parent);
        canvasGO.transform.localPosition = Vector3.zero;
        canvasGO.transform.localScale = Vector3.one * 0.005f; // Fixed reasonable scale
        
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingLayerName = "Default";
        canvas.sortingOrder = 100;
        
        // Remove CanvasScaler - keep it simple
        
        // Create Panel
        GameObject panelGO = new GameObject("TooltipPanel");
        panelGO.transform.SetParent(canvasGO.transform);
        
        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(1, 1); // Unit size - will scale with canvas
        
        // Create Background Image
        backgroundImage = panelGO.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark semi-transparent
        backgroundImage.type = Image.Type.Sliced;
        
        // Create Vertical Layout Group - simplified
        VerticalLayoutGroup layout = panelGO.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 3;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        
        // Remove ContentSizeFitter - use fixed size instead
        
        // Create Text Elements with smaller, fixed sizes
        nameText = CreateTextElement(panelGO.transform, "NameText", "Potion Name", 14, FontStyles.Bold, Color.white);
        levelText = CreateTextElement(panelGO.transform, "LevelText", "Level 1.1", 11, FontStyles.Normal, Color.yellow);
        descriptionText = CreateTextElement(panelGO.transform, "DescriptionText", "Potion description", 9, FontStyles.Normal, Color.gray);
        durationText = CreateTextElement(panelGO.transform, "DurationText", "Duration: 30s", 9, FontStyles.Normal, Color.cyan);
        
        panel = panelGO;
    }
    
    /// <summary>
    /// Create a text element with specified properties
    /// </summary>
    private static TextMeshProUGUI CreateTextElement(Transform parent, string name, string text, 
        float fontSize, FontStyles fontStyle, Color color)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(230, 20); // Fixed smaller size
        
        TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = fontStyle;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Left;
        textComponent.enableWordWrapping = true;
        
        // Simple LayoutElement with fixed height
        LayoutElement layoutElement = textGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = fontSize + 2;
        layoutElement.flexibleWidth = 1;
        
        return textComponent;
    }
}
