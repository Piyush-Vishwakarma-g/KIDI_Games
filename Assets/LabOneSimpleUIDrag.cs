using UnityEngine;
using UnityEngine.EventSystems;

public class LabOneSimpleUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private RectTransform dragParent;
    private Vector2 startPosition;
    private Vector2 previousPointerPosition;
    private Camera mainCamera;
    private Canvas parentCanvas;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"'{gameObject.name}' needs a RectTransform to be UI-draggable.", this);
            enabled = false;
            return;
        }

        mainCamera = Camera.main;
        parentCanvas = GetComponentInParent<Canvas>();
        dragParent = rectTransform.parent as RectTransform;

        if (parentCanvas == null || dragParent == null)
        {
            Debug.LogError($"'{gameObject.name}' must be under a Canvas with a RectTransform parent.", this);
            enabled = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TryGetPointerLocalPosition(eventData, out Vector2 pointerPosition))
        {
            return;
        }

        startPosition = rectTransform.anchoredPosition;
        previousPointerPosition = pointerPosition;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !TryGetPointerLocalPosition(eventData, out Vector2 pointerPosition))
        {
            return;
        }

        rectTransform.anchoredPosition += pointerPosition - previousPointerPosition;
        previousPointerPosition = pointerPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        try
        {
            Camera raycastCamera = mainCamera != null ? mainCamera : Camera.main;

            if (raycastCamera == null)
            {
                Debug.LogError($"Cannot raycast dropped UI '{gameObject.name}': no camera is available.");
            }
            else
            {
                Ray ray = raycastCamera.ScreenPointToRay(eventData.position);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                RaycastHit? matchingHit = null;

                foreach (RaycastHit hit in hits)
                {
                    Debug.Log($"Dragged UI '{gameObject.name}' raycast hit collider '{hit.collider.name}' with tag '{hit.collider.tag}'.");

                    if (hit.collider.CompareTag(transform.name) &&
                        (!matchingHit.HasValue || hit.distance < matchingHit.Value.distance))
                    {
                        matchingHit = hit;
                    }
                }

                if (matchingHit.HasValue)
                {
                    Collider targetCollider = matchingHit.Value.collider;
                    Debug.Log($"Correct drop: UI '{gameObject.name}' hit collider '{targetCollider.name}'.");

                    if (targetCollider.transform.childCount > 0)
                        targetCollider.transform.GetChild(0).gameObject.SetActive(true);

                    if (Lab4Manager.instance != null)
                        Lab4Manager.instance.CheckAllUnableObjects();
                    else if (ChemistryLab1Manager.instance != null)
                        ChemistryLab1Manager.instance.CheckAllUnableObjects();
                }
                else if (hits.Length == 0)
                {
                    Debug.Log($"Dragged UI '{gameObject.name}' raycast did not hit any collider.");
                }
                else
                {
                    Debug.Log($"Wrong drop: UI '{gameObject.name}' did not hit a collider tagged '{transform.name}'.");
                }
            }
        }
        finally
        {
            isDragging = false;
            ResetColliders();
            rectTransform.anchoredPosition = startPosition;
        }
    }

    private bool TryGetPointerLocalPosition(PointerEventData eventData, out Vector2 localPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragParent,
                eventData.position,
                eventData.pressEventCamera,
                out localPosition))
        {
            return true;
        }

        Debug.LogWarning($"Could not map the pointer to the Canvas plane for '{gameObject.name}'.", this);
        return false;
    }

    public void ShowGreatJob()
    {
        
    }

    private void ResetColliders()
    {
        if (GameManager.instance != null)
            GameManager.instance.EndUiDrag();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ShowGreatJob));
        ResetColliders();
    }
}