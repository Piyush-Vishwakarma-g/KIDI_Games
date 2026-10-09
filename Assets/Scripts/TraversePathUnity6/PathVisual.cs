using System.Collections.Generic;
using UnityEngine;

public class PathVisual : MonoBehaviour
{
    public LineRenderer glowLine;
    public Transform dotsRoot;
    public float dotSpacing = 0.28f;
    public float dotSize = 0.075f;

    private readonly List<Transform> dots = new();
    private Material lineMaterial;
    private Sprite dotSprite;

    public void Build(List<Vector3> points, Color baseColor)
    {
        ClearDots();

        if (glowLine == null)
            glowLine = CreateLine("GlowPath", transform);

        lineMaterial = CreateMaterial();
        glowLine.material = lineMaterial;
        glowLine.startWidth = 0.075f;
        glowLine.endWidth = 0.075f;
        glowLine.positionCount = points.Count;
        glowLine.numCapVertices = 8;
        glowLine.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.10f);
        glowLine.endColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.10f);
        glowLine.useWorldSpace = true;
        glowLine.SetPositions(points.ToArray());

        dotSprite = CreateCircleSprite(64);
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[i + 1];
            float distance = Vector3.Distance(a, b);
            int count = Mathf.Max(1, Mathf.FloorToInt(distance / dotSpacing));

            for (int j = 0; j < count; j++)
            {
                float t = (j + 0.5f) / count;
                CreateDot(Vector3.Lerp(a, b, t), baseColor);
            }
        }
    }

    public void SetProgress(float normalized, Color activeColor)
    {
        if (glowLine == null) return;

        float alpha = Mathf.Lerp(0.10f, 0.95f, normalized);
        glowLine.startColor = new Color(activeColor.r, activeColor.g, activeColor.b, alpha);
        glowLine.endColor = new Color(activeColor.r, activeColor.g, activeColor.b, alpha);

        int activeDots = Mathf.RoundToInt(dots.Count * normalized);
        for (int i = 0; i < dots.Count; i++)
        {
            SpriteRenderer sr = dots[i].GetComponent<SpriteRenderer>();
            if (sr == null) continue;
            sr.color = i < activeDots
                ? new Color(activeColor.r, activeColor.g, activeColor.b, 1f)
                : new Color(activeColor.r, activeColor.g, activeColor.b, 0.32f);
        }
    }

    public void FlashWrong(Color red)
    {
        if (glowLine == null) return;
        glowLine.startColor = new Color(red.r, red.g, red.b, 1f);
        glowLine.endColor = new Color(red.r, red.g, red.b, 1f);
        Invoke(nameof(Restore), 0.20f);
    }

    private void Restore()
    {
        if (glowLine == null) return;
        glowLine.startColor = new Color(0.2f, 1f, 0.35f, 0.25f);
        glowLine.endColor = new Color(0.2f, 1f, 0.35f, 0.25f);
    }

    private void CreateDot(Vector3 position, Color color)
    {
        GameObject go = new GameObject("PathDot");
        go.transform.SetParent(dotsRoot != null ? dotsRoot : transform);
        go.transform.position = position;
        go.transform.localScale = Vector3.one * dotSize;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = dotSprite;
        sr.color = new Color(color.r, color.g, color.b, 0.32f);
        sr.sortingOrder = 5;
        dots.Add(go.transform);
    }

    private void ClearDots()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] != null) Destroy(dots[i].gameObject);
        }
        dots.Clear();
    }

    private LineRenderer CreateLine(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        return go.AddComponent<LineRenderer>();
    }

    private Material CreateMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        return new Material(shader);
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[size * size];
        float center = (size - 1) * 0.5f;
        float radius = center - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                pixels[y * size + x] = dx * dx + dy * dy <= radius * radius
                    ? Color.white
                    : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
