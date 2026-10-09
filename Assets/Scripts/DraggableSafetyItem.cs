using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSafetyItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType { Goggles, Gloves }
    public ItemType itemType;

    [HideInInspector] public bool isEquipped = false;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEquipped) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEquipped || canvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEquipped) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        if (!isEquipped) rectTransform.position = originalPosition;
    }

    public void EquipToTarget(Vector3 targetPosition)
    {
        isEquipped = true;
        rectTransform.position = targetPosition;
        canvasGroup.blocksRaycasts = false;
    }
}