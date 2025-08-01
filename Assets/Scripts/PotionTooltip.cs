using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tooltip component that displays potion information in a World Space Canvas
/// Automatically positioned to the left of the potion
/// </summary>
public class PotionTooltip : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas tooltipCanvas;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Positioning")]
    [SerializeField] private Vector3 offset = new Vector3(-1.5f, 0f, 0f); // Will be calculated based on potion height
    [SerializeField] private float backgroundPadding = 0.2f;
    
    private AnimatedPotion potionData;
    private bool isVisible = false;
    
    void Awake()
    {
        // Ensure the tooltip starts hidden
        tooltipPanel.SetActive(false);
    }
    
    /// <summary>
    /// Initialize the tooltip with potion data reference
    /// </summary>
    public void Initialize(AnimatedPotion potion)
    {
        potionData = potion;
        
        // Create tooltip UI automatically if not assigned
        if (tooltipCanvas == null || tooltipPanel == null)
        {
            CreateTooltipUI();
        }
    }
    
    /// <summary>
    /// Show the tooltip with current potion data
    /// </summary>
    public void ShowTooltip()
    {
        if (potionData == null || tooltipPanel == null) return;
        
        UpdateTooltipContent();
        tooltipPanel.SetActive(true);
        isVisible = true;
    }
    
    /// <summary>
    /// Hide the tooltip
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        isVisible = false;
    }
    
    /// <summary>
    /// Update tooltip content with current potion data
    /// </summary>
    private void UpdateTooltipContent()
    {
        if (potionData == null) return;
        
        // Update text fields
        if (nameText != null)
            nameText.text = potionData.potionName;
            
        if (levelText != null)
            levelText.text = $"Level {potionData.level}.{potionData.subLevel}";
            
        if (descriptionText != null)
            descriptionText.text = potionData.description;
            
        if (durationText != null)
            durationText.text = $"Duration: {potionData.duration}s";
    }
    
    /// <summary>
    /// Toggle tooltip visibility
    /// </summary>
    public void ToggleTooltip()
    {
        if (isVisible)
            HideTooltip();
        else
            ShowTooltip();
    }
    
    /// <summary>
    /// Check if tooltip is currently visible
    /// </summary>
    public bool IsVisible => isVisible;
    
    /// <summary>
    /// Update tooltip offset position
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        tooltipCanvas.transform.localPosition = offset;
    }
    
    /// <summary>
    /// Create the tooltip UI automatically
    /// </summary>
    private void CreateTooltipUI()
    {
        // Create main canvas as child of the potion
        GameObject canvasGO = new GameObject("TooltipCanvas");
        canvasGO.transform.SetParent(transform, false);
        
        tooltipCanvas = canvasGO.AddComponent<Canvas>();
        tooltipCanvas.renderMode = RenderMode.WorldSpace;
        tooltipCanvas.sortingOrder = 10;
        
        // Set a very small scale for the entire canvas
        canvasGO.transform.localScale = Vector3.one * 0.02f;
        
        // Set canvas anchor to the right so it positions from its right edge
        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.anchorMin = new Vector2(1f, 0.5f); // Right center
        canvasRect.anchorMax = new Vector2(1f, 0.5f); // Right center
        canvasRect.pivot = new Vector2(1f, 0.5f);     // Pivot on right center
        
        // Create background panel
        GameObject panelGO = new GameObject("TooltipPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        tooltipPanel = panelGO;
        
        backgroundImage = panelGO.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(200, 120);
        
        // Create vertical layout group
        VerticalLayoutGroup layout = panelGO.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 1f;
        layout.padding = new RectOffset(3, 3, 3, 3);
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        ContentSizeFitter fitter = panelGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Create text elements
        nameText = CreateTextElement("NameText", panelGO.transform, 14, Color.white, true);
        levelText = CreateTextElement("LevelText", panelGO.transform, 12, Color.yellow, false);
        descriptionText = CreateTextElement("DescriptionText", panelGO.transform, 10, Color.gray, false);
        durationText = CreateTextElement("DurationText", panelGO.transform, 10, Color.cyan, false);
        
        // Position canvas to the left of the potion
        Vector3 potionHeight = Vector3.zero;
        offset = new Vector3(-0.5f, 0.5f, 0f);
        canvasGO.transform.localPosition = offset;
        
        // Start hidden
        tooltipPanel.SetActive(false);
    }
    
    /// <summary>
    /// Helper method to create text elements
    /// </summary>
    private TextMeshProUGUI CreateTextElement(string name, Transform parent, int fontSize, Color color, bool bold)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = name;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        text.alignment = TextAlignmentOptions.Left;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(180, fontSize + 5);
        
        return text;
    }
}
