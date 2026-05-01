using UnityEngine;
using UnityEditor;

// Automatically adjust any texture file in folder "Assets/Sprites/"
// in its file name to have the correct properties.

class MyTexturePostprocessor : AssetPostprocessor
{
    // Increment the version number, when the AssetPostprocessors code/behavior is changed
    static readonly uint k_Version = 0;
    public override uint GetVersion() { return k_Version; }

    void OnPreprocessTexture()
    {
        if (assetPath.Contains("Assets/Sprites/"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.spritePixelsPerUnit = 16;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}