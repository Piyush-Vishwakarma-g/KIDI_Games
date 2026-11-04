
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lab4SimpleUIDrag : MonoBehaviour
{
    private static Lab4SimpleUIDrag activePointerDrag;
    private static readonly List<RaycastResult> raycastResults =
        new List<RaycastResult>();

    private RectTransform rectTransform;
    private RectTransform dragParent;
    private Canvas parentCanvas;

    private Vector2 startPosition;
    private Vector2 previousPointerLocalPosition;

    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (rectTransform == null)
        {
            Debug.LogError(
                $"'{gameObject.name}' needs a RectTransform.", this);
            enabled = false;
            return;
        }

        if (parentCanvas == null)
        {
            Debug.LogError(
                $"'{gameObject.name}' must be inside a Canvas.", this);
            enabled = false;
            return;
        }

        dragParent = rectTransform.parent as RectTransform;

        if (dragParent == null)
        {
            Debug.LogError(
                $"'{gameObject.name}' needs a RectTransform parent.", this);
            enabled = false;
            return;
        }

        if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay &&
            parentCanvas.worldCamera == null)
        {
            Debug.LogError(
                $"Assign a Render Camera to Canvas '{parentCanvas.name}'.",
                parentCanvas);
        }
    }

    private void Start()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("No active EventSystem found.", this);
        }

        if (parentCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            Debug.LogError(
                $"Canvas '{parentCanvas.name}' needs a GraphicRaycaster.",
                parentCanvas);
        }

        Graphic graphic = GetComponent<Graphic>();

        if (graphic == null || !graphic.raycastTarget)
        {
            Debug.LogError(
                $"Enable Raycast Target on '{gameObject.name}'.", this);
        }
    }

    private void Update()
    {
        // Read mouse/touch input using the New Input System.
        Pointer pointer = Pointer.current;

        if (pointer == null)
            return;

        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
            return;

        Vector2 screenPosition = pointer.position.ReadValue();

        PointerEventData eventData = new PointerEventData(eventSystem)
        {
            position = screenPosition,
            delta = pointer.delta.ReadValue(),
            button = PointerEventData.InputButton.Left
        };

        if (pointer.press.wasPressedThisFrame)
        {
            // Find the topmost UI element under the pointer.
            raycastResults.Clear();
            eventSystem.RaycastAll(eventData, raycastResults);

            activePointerDrag = null;

            foreach (RaycastResult result in raycastResults)
            {
                Lab4SimpleUIDrag candidate =
                    result.gameObject.GetComponentInParent<Lab4SimpleUIDrag>();

                if (candidate != null && candidate.isActiveAndEnabled)
                {
                    activePointerDrag = candidate;
                    break;
                }
            }

            if (activePointerDrag != null)
            {
                activePointerDrag.BeginDrag(eventData);
            }
            else
            {
                Debug.Log("No draggable UI found under pointer.");
            }
        }

        if (activePointerDrag != null &&
            pointer.press.isPressed &&
            !pointer.press.wasPressedThisFrame)
        {
            activePointerDrag.Drag(eventData);
        }

        if (activePointerDrag != null &&
            pointer.press.wasReleasedThisFrame)
        {
            Lab4SimpleUIDrag draggedObject = activePointerDrag;
            activePointerDrag = null;

            draggedObject.Drag(eventData);
            draggedObject.EndDrag(eventData);
        }
    }

    private void BeginDrag(PointerEventData eventData)
    {
        if (!TryGetPointerLocalPosition(
            eventData.position, out previousPointerLocalPosition))
        {
            Debug.LogWarning(
                $"Could not get pointer position for '{gameObject.name}'.",
                this);
            return;
        }

        startPosition = rectTransform.anchoredPosition;
        isDragging = true;

        PrepareDropColliders();

        Debug.Log($"Started dragging '{gameObject.name}'.", this);
    }

    private void Drag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        if (TryGetPointerLocalPosition(
            eventData.position, out Vector2 pointerLocalPosition))
        {
            Vector2 difference =
                pointerLocalPosition - previousPointerLocalPosition;

            rectTransform.anchoredPosition += difference;
            previousPointerLocalPosition = pointerLocalPosition;
        }
    }

    private void EndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        try
        {
            Camera raycastCamera = parentCanvas.worldCamera;

            if (raycastCamera == null &&
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                raycastCamera = Camera.main;
            }

            if (raycastCamera == null)
            {
                Debug.LogError(
                    "Drop detection needs a camera. Assign the Canvas Render Camera.",
                    this);
                return;
            }

            Ray ray = raycastCamera.ScreenPointToRay(eventData.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            RaycastHit? matchingHit = null;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null &&
                    hit.collider.CompareTag(gameObject.name) &&
                    (!matchingHit.HasValue ||
                     hit.distance < matchingHit.Value.distance))
                {
                    matchingHit = hit;
                }
            }

            if (matchingHit.HasValue)
            {
                Collider targetCollider = matchingHit.Value.collider;

                Debug.Log(
                    $"Correct drop: '{gameObject.name}' on '{targetCollider.name}'.",
                    targetCollider);

                if (targetCollider.transform.childCount > 0)
                {
                    targetCollider.transform.GetChild(0)
                        .gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning(
                        $"'{targetCollider.name}' has no child indicator.",
                        targetCollider);
                }

                if (Lab4Manager.instance != null)
                {
                    Lab4Manager.instance.CheckAllUnableObjects();
                }
                else
                {
                    Debug.LogWarning("Lab4Manager.instance is null.", this);
                }
            }
            else
            {
                Debug.Log("No matching drop target found.", this);
            }
        }
        finally
        {
            isDragging = false;

            // Restore colliders after the drop.
            ResetColliders();

            // Return UI to its original position, as in your old script.
            rectTransform.anchoredPosition = startPosition;
        }
    }

    private bool TryGetPointerLocalPosition(
        Vector2 screenPosition, out Vector2 localPosition)
    {
        Camera canvasCamera =
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragParent,
            screenPosition,
            canvasCamera,
            out localPosition);
    }

    private void PrepareDropColliders()
    {
        GameManager gameManager = GameManager.instance;

        if (gameManager == null)
            return;

        gameManager.isDraggingUI = true;

        if (gameManager.levelCubeObj != null &&
            LevelData.instance != null)
        {
            int index = LevelData.instance.levelIndex - 1;

            if (index >= 0 && index < gameManager.levelCubeObj.Length)
            {
                GameObject cube = gameManager.levelCubeObj[index];

                if (cube != null)
                {
                    BoxCollider cubeCollider =
                        cube.GetComponent<BoxCollider>();

                    if (cubeCollider != null)
                        cubeCollider.enabled = false;
                }
            }
        }

        if (gameManager.sidePhasesCollider != null)
        {
            foreach (BoxCollider sidePhase in
                     gameManager.sidePhasesCollider)
            {
                if (sidePhase != null)
                    sidePhase.enabled = true;
            }
        }
    }

    public void ShowGreatJob()
    {
        // Add your success animation or message here if needed.
    }

    private void ResetColliders()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.EndUiDrag();
        }
    }

    private void OnDisable()
    {
        if (activePointerDrag == this)
            activePointerDrag = null;

        isDragging = false;
        CancelInvoke(nameof(ShowGreatJob));
        ResetColliders();
    }
}
