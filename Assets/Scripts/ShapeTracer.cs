using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeTracer : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Setup Nodes")]
    public RectTransform[] traceNodes;
    public RectTransform tracingPointer; 
    public float nodeProximityTolerance = 65f;
    public float maxOffPathDistance = 100f;

    [Header("UI Line Customization")]
    public Color lineColor = new Color(0.96f, 0.31f, 0.64f); // Premium Pink Glow
    public float lineThickness = 12f;

    private int currentTargetNodeIndex = 0;
    private bool isTracingActive = false;
    private List<RectTransform> activeUiLines = new List<RectTransform>();
    private RectTransform liveUiLine;
    private Vector2 lastAnchoredPosition;
    private RectTransform parentRect;

    void Start()
    {
        parentRect = transform as RectTransform;
        ResetTracing();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out localMousePos);
        
        if (Vector2.Distance(localMousePos, traceNodes[0].anchoredPosition) <= nodeProximityTolerance)
        {
            isTracingActive = true;
            currentTargetNodeIndex = 1;
            ClearUiLines();
            lastAnchoredPosition = traceNodes[0].anchoredPosition;
            
            // Create a temporary live line following mouse drag
            liveUiLine = CreateLineSegment(lastAnchoredPosition, lastAnchoredPosition);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTracingActive) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out localMousePos);

        if (tracingPointer != null) tracingPointer.anchoredPosition = localMousePos;

        // Check if player reached the next node corner
        float distanceToTarget = Vector2.Distance(localMousePos, traceNodes[currentTargetNodeIndex].anchoredPosition);
        if (distanceToTarget <= nodeProximityTolerance)
        {
            // Lock down a permanent line till this node corner
            UpdateLineVisual(liveUiLine, lastAnchoredPosition, traceNodes[currentTargetNodeIndex].anchoredPosition);
            activeUiLines.Add(liveUiLine);

            lastAnchoredPosition = traceNodes[currentTargetNodeIndex].anchoredPosition;

            if (currentTargetNodeIndex < traceNodes.Length - 1)
            {
                currentTargetNodeIndex++;
                // Start a brand new live line for next segment
                liveUiLine = CreateLineSegment(lastAnchoredPosition, localMousePos);
            }
            else
            {
                OnTracingComplete();
            }
            return;
        }

        // Check if player is sliding off the legal path segment
        float offPath = FindDistanceToSegment(localMousePos, traceNodes[currentTargetNodeIndex - 1].anchoredPosition, traceNodes[currentTargetNodeIndex].anchoredPosition);
        if (offPath > maxOffPathDistance)
        {
            ResetTracing();
        }
        else
        {
            // Continuously update live visual path following player drag pointer
            if (liveUiLine != null)
            {
                UpdateLineVisual(liveUiLine, lastAnchoredPosition, localMousePos);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetTracing();
    }

    private RectTransform CreateLineSegment(Vector2 from, Vector2 to)
    {
        GameObject lineObj = new GameObject("UI_Line_Segment", typeof(RectTransform), typeof(Image));
        lineObj.transform.SetParent(this.transform, false);
        lineObj.transform.SetAsLastSibling(); // Isse line sabse upar front mein dikhegi
// Draw behind the corner node badges

        Image img = lineObj.GetComponent<Image>();
        img.color = lineColor;
        
        RectTransform rt = lineObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        
        UpdateLineVisual(rt, from, to);
        return rt;
    }

    private void UpdateLineVisual(RectTransform rt, Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.anchoredPosition = from;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
        
        // Pivot set to left center edge so rotation handles properly from point A
        rt.pivot = new Vector2(0f, 0.5f); 
    }

    private void ClearUiLines()
    {
        foreach (var line in activeUiLines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        activeUiLines.Clear();
        if (liveUiLine != null)
        {
            Destroy(liveUiLine.gameObject);
            liveUiLine = null;
        }
    }

    void ResetTracing()
    {
        isTracingActive = false;
        currentTargetNodeIndex = 0;
        ClearUiLines();
    }

    void OnTracingComplete()
    {
        isTracingActive = false;
        if (liveUiLine != null) Destroy(liveUiLine.gameObject);
        Debug.Log("Shape Completed!");
    }

    private float FindDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a, ap = p - a;
        float l2 = ab.sqrMagnitude;
        if (l2 == 0) return Vector2.Distance(p, a);
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / l2);
        return Vector2.Distance(p, a + t * ab);
    }
}
