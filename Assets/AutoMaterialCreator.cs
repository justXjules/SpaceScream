using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoMaterialCreator : Editor
{
    [MenuItem("Assets/Create Materials from Textures", false, 300)]
    static void CreateMaterialsFromSelectedTextures()
    {
        // Get all selected textures
        Object[] selectedObjects = Selection.objects;
        int materialsCreated = 0;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            
            // Only process texture files
            if (!assetPath.EndsWith(".png") && !assetPath.EndsWith(".jpg") && !assetPath.EndsWith(".tga"))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string directory = Path.GetDirectoryName(assetPath);
            string materialPath = Path.Combine(directory, fileName + ".mat");

            // Skip if material already exists
            if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) != null)
            {
                Debug.Log($"Material already exists: {fileName}.mat - Skipping");
                continue;
            }

            // Skip normal/metallic/AO maps
            if (fileName.EndsWith("_normal", System.StringComparison.OrdinalIgnoreCase) || 
                fileName.EndsWith("_Normal") ||
                fileName.EndsWith("_metallic", System.StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith("_Metallic") ||
                fileName.EndsWith("_AO") ||
                fileName.EndsWith("_ao", System.StringComparison.OrdinalIgnoreCase))
                continue;

            // Create new material
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // Load and assign the texture
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture != null)
            {
                material.SetTexture("_BaseMap", texture);
            }

            // Create the material
            AssetDatabase.CreateAsset(material, materialPath);
            materialsCreated++;
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"Created {materialsCreated} materials");
    }

    // Only show the menu option when textures are selected
    [MenuItem("Assets/Create Materials from Textures", true)]
    static bool ValidateCreateMaterials()
    {
        return Selection.objects.Length > 0;
    }
}