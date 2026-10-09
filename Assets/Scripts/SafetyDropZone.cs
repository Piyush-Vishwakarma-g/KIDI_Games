using UnityEngine;
using UnityEngine.EventSystems;

public class SafetyDropZone : MonoBehaviour, IDropHandler
{
    public enum EquipmentType { Goggles, Gloves }
    public EquipmentType requiredType;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableSafetyItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableSafetyItem>();
        if (draggedItem != null && !draggedItem.isEquipped)
        {
            if (draggedItem.itemType.ToString() == requiredType.ToString())
            {
                draggedItem.EquipToTarget(transform.position);
                if (SafetyManager.Instance != null)
                {
                    SafetyManager.Instance.EquipItem(requiredType.ToString());
                }
            }
        }
    }
}