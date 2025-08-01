using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PotionAnimationDatabase))]
public class PotionAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Dessiner l'interface par défaut
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        PotionAnimationDatabase database = (PotionAnimationDatabase)target;
        
        // Bouton pour initialiser les 7 types de potions magiques
        if (GUILayout.Button("Initialize 7 Magic Types", GUILayout.Height(30)))
        {
            database.InitializeMagicTypes();
            EditorUtility.SetDirty(database); // Marquer comme modifié pour sauvegarder
        }
        
        EditorGUILayout.Space();
        
        // Afficher des informations utiles
        if (database.magicTypes != null && database.magicTypes.Length > 0)
        {
            EditorGUILayout.HelpBox($"Database contains {database.magicTypes.Length} magic potion types.", MessageType.Info);
            
            // Compter combien ont des controllers assignés
            int assignedCount = 0;
            foreach (var mapping in database.magicTypes)
            {
                if (mapping != null && mapping.animatorController != null)
                    assignedCount++;
            }
            
            EditorGUILayout.HelpBox($"{assignedCount} out of {database.magicTypes.Length} magic types have animation controllers assigned.", 
                assignedCount == database.magicTypes.Length ? MessageType.Info : MessageType.Warning);
                
            // Afficher la logique des controllers
            EditorGUILayout.HelpBox(
                "Controller Logic:\n" +
                "1. Plant only\n" +
                "2. Animal only\n" +
                "3. Mineral only\n" +
                "4. Plant + Animal\n" +
                "5. Plant + Mineral\n" +
                "6. Animal + Mineral\n" +
                "7. Plant + Animal + Mineral", 
                MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("No magic types found. Click 'Initialize 7 Magic Types' to create them.", MessageType.Warning);
        }
    }
}
