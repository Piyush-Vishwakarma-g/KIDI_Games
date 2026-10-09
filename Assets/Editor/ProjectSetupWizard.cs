using UnityEngine;
using UnityEditor;
using System.IO;

public class ProjectSetupWizard : EditorWindow
{
    [MenuItem("Tools/Build Shape Tracing Game")]
    public static void BuildProject()
    {
        string assetsPath = Application.dataPath;

        // 1. Generate Folder Architecture
        CreateDirectory(Path.Combine(assetsPath, "Scripts"));
        CreateDirectory(Path.Combine(assetsPath, "Prefabs"));
        CreateDirectory(Path.Combine(assetsPath, "Sprites"));
        CreateDirectory(Path.Combine(assetsPath, "Scenes"));

        // 2. Auto-Generate Game Logic Script
        string scriptPath = Path.Combine(assetsPath, "Scripts", "ShapeTracer.cs");
        if (!File.Exists(scriptPath))
        {
            File.WriteAllText(scriptPath, GetShapeTracerCode());
            Debug.Log("✅ Created: ShapeTracer.cs inside Scripts folder.");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Project Builder", "Project structure and source code generated successfully!", "OK");
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"📁 Created Folder: {path}");
        }
    }

    private static string GetShapeTracerCode()
    {
        return @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeTracer : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform[] traceNodes;
    public LineRenderer lineRenderer;
    public RectTransform tracingPointer;
    public float nodeProximityTolerance = 50f;
    public float maxOffPathDistance = 80f;

    private int currentTargetNodeIndex = 0;
    private bool isTracingActive = false;
    private List<Vector3> drawnPoints = new List<Vector3>();

    void Start()
    {
        ResetTracing();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(traceNodes[0].parent as RectTransform, eventData.position, eventData.pressEventCamera, out localMousePos);
        if (Vector2.Distance(localMousePos, traceNodes[0].anchoredPosition) <= nodeProximityTolerance)
        {
            isTracingActive = true;
            currentTargetNodeIndex = 1;
            drawnPoints.Clear();
            AddLinePoint(traceNodes[0].position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTracingActive) return;
        Vector2 localMousePos;
        RectTransform parentRect = traceNodes[0].parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out localMousePos);

        if (tracingPointer != null) tracingPointer.anchoredPosition = localMousePos;

        float distanceToTarget = Vector2.Distance(localMousePos, traceNodes[currentTargetNodeIndex].anchoredPosition);
        if (distanceToTarget <= nodeProximityTolerance)
        {
            AddLinePoint(traceNodes[currentTargetNodeIndex].position);
            if (currentTargetNodeIndex < traceNodes.Length - 1) currentTargetNodeIndex++;
            else OnTracingComplete();
            return;
        }

        float offPath = FindDistanceToSegment(localMousePos, traceNodes[currentTargetNodeIndex - 1].anchoredPosition, traceNodes[currentTargetNodeIndex].anchoredPosition);
        if (offPath > maxOffPathDistance) ResetTracing();
        else
        {
            Vector3 worldDragPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out worldDragPos);
            if (lineRenderer != null && drawnPoints.Count > 0)
            {
                lineRenderer.positionCount = drawnPoints.Count + 1;
                lineRenderer.SetPosition(drawnPoints.Count, worldDragPos);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) => ResetTracing();

    void AddLinePoint(Vector3 worldPos)
    {
        drawnPoints.Add(worldPos);
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = drawnPoints.Count;
            lineRenderer.SetPosition(drawnPoints.Count - 1, worldPos);
        }
    }

    void ResetTracing()
    {
        isTracingActive = false;
        currentTargetNodeIndex = 0;
        if (lineRenderer != null) lineRenderer.positionCount = 0;
        drawnPoints.Clear();
    }

    void OnTracingComplete()
    {
        isTracingActive = false;
        Debug.Log('Shape Completed!');
    }

    private float FindDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a, ap = p - a;
        float l2 = ab.sqrMagnitude;
        if (l2 == 0) return Vector2.Distance(p, a);
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / l2);
        return Vector2.Distance(p, a + t * ab);
    }
}";
    }
}
