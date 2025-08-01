using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Editor script to directly modify .meta files and change sprite alignment to Bottom (7)
/// This is much faster than using TextureImporter.SaveAndReimport()
/// </summary>
public class MetaFilePivotChanger : EditorWindow
{
    private string folderPath = "Assets/karsiori/Pixel Potion Pack - Animated/Sprites";
    
    [MenuItem("Tools/Change Meta Files Pivot to Bottom")]
    public static void ShowWindow()
    {
        GetWindow<MetaFilePivotChanger>("Meta File Pivot Changer");
    }

    void OnGUI()
    {
        GUILayout.Label("Meta File Pivot Changer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool directly modifies .meta files (much faster!)");
        GUILayout.Space(5);
        
        GUILayout.Label("Folder path:");
        folderPath = EditorGUILayout.TextField(folderPath);
        
        GUILayout.Space(5);
        GUILayout.Label("Will change: alignment: 0 → alignment: 7 (Bottom)", EditorStyles.miniLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Browse Folder", GUILayout.Height(25)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Change All PNG Meta Files to Bottom Pivot", GUILayout.Height(40)))
        {
            ChangeMetaFiles(folderPath);
        }
    }

    static void ChangeMetaFiles(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("Error", "Please specify a folder path!", "OK");
            return;
        }

        // Convert Unity path to absolute path
        string absoluteFolderPath = Path.Combine(Application.dataPath, folderPath.Substring("Assets/".Length));
        
        if (!Directory.Exists(absoluteFolderPath))
        {
            EditorUtility.DisplayDialog("Error", $"Folder '{folderPath}' does not exist!", "OK");
            return;
        }

        // Find all .png.meta files recursively
        string[] metaFiles = Directory.GetFiles(absoluteFolderPath, "*.png.meta", SearchOption.AllDirectories);
        
        int changedCount = 0;
        int totalCount = metaFiles.Length;

        foreach (string metaFilePath in metaFiles)
        {
            if (ModifyMetaFile(metaFilePath))
            {
                changedCount++;
                // Convert back to relative path for logging
                string relativePath = "Assets" + metaFilePath.Substring(Application.dataPath.Length);
                Debug.Log($"Modified: {relativePath}");
            }
        }

        // Refresh Unity to reload the modified meta files
        AssetDatabase.Refresh();
        
        Debug.Log($"Processed {totalCount} .png.meta files. Modified {changedCount} files.");
        
        EditorUtility.DisplayDialog("Success", 
            $"Processed {totalCount} .png.meta files.\nModified {changedCount} files to Bottom pivot!", "OK");
    }

    static bool ModifyMetaFile(string metaFilePath)
    {
        try
        {
            string content = File.ReadAllText(metaFilePath);
            string originalContent = content;
            
            // Change alignment from any value to 7 (Bottom)
            // Pattern: "alignment: " followed by any number
            content = Regex.Replace(content, @"alignment: \d+", "alignment: 7");
            
            // If content changed, write it back
            if (content != originalContent)
            {
                File.WriteAllText(metaFilePath, content);
                return true;
            }
            
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error modifying {metaFilePath}: {ex.Message}");
            return false;
        }
    }
}
