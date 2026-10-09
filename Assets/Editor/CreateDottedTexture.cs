using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateDottedTexture
{
    [MenuItem("Tools/Generate Dotted Texture")]
    public static void GenerateTexture()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        // Fill background with transparent black
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        tex.SetPixels(pixels);

        // Draw a white circle in the center
        float center = size / 2f;
        float radius = size / 3.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radius)
                {
                    // Soft anti-aliased edge
                    float alpha = Mathf.SmoothStep(1f, 0f, (dist - (radius - 1f)));
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
        }

        tex.Apply();

        // Encode to PNG and save to Assets
        byte[] bytes = tex.EncodeToPNG();
        string path = Application.dataPath + "/DottedTexture.png";
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        Debug.Log("DottedTexture.png generated at Assets/DottedTexture.png");
    }
}