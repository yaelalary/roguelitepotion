#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ingredient))]
public class IngredientEditor : Editor
{
    private SerializedProperty ingredientNameProp;
    private SerializedProperty familyProp;
    private SerializedProperty subFamilyProp;
    private SerializedProperty descriptionProp;
    private SerializedProperty iconProp;
    
    private int selectedNameIndex = 0;
    private string[] availableNames;
    private string[] displayNames;
    
    void OnEnable()
    {
        // Get serialized properties
        ingredientNameProp = serializedObject.FindProperty("ingredientName");
        familyProp = serializedObject.FindProperty("family");
        subFamilyProp = serializedObject.FindProperty("subFamily");
        descriptionProp = serializedObject.FindProperty("description");
        iconProp = serializedObject.FindProperty("icon");
        
        // Setup dropdown options
        availableNames = IngredientNames.GetAllIngredientNames();
        displayNames = new string[availableNames.Length];
        
        for (int i = 0; i < availableNames.Length; i++)
        {
            displayNames[i] = IngredientNames.GetDisplayName(availableNames[i]);
        }
        
        // Find current selection index
        string currentName = ingredientNameProp.stringValue;
        selectedNameIndex = System.Array.IndexOf(availableNames, currentName);
        if (selectedNameIndex < 0) selectedNameIndex = 0;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ingredient Settings", EditorStyles.boldLabel);
        
        // Ingredient Name Dropdown
        EditorGUI.BeginChangeCheck();
        selectedNameIndex = EditorGUILayout.Popup("Ingredient Name", selectedNameIndex, displayNames);
        if (EditorGUI.EndChangeCheck())
        {
            // Store the internal constant name, not the display name
            ingredientNameProp.stringValue = availableNames[selectedNameIndex];
        }
        
        // Show the actual string value (read-only)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Internal Name", ingredientNameProp.stringValue);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        
        // Other properties
        EditorGUILayout.PropertyField(familyProp);
        EditorGUILayout.PropertyField(subFamilyProp);
        EditorGUILayout.PropertyField(descriptionProp);
        EditorGUILayout.PropertyField(iconProp);
        
        serializedObject.ApplyModifiedProperties();
        
        // Helper info
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("The ingredient name is restricted to predefined constants to ensure consistency across the game.", MessageType.Info);
    }
}
#endif
