using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndReturnObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 offset;
    private float zDistance;

    void Start()
    {
        mainCamera = Camera.main;
        // Apni original position aur rotation save kar rahe hain
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Object ki depth (Z distance) camera se calculate karte hain
        zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        
        Vector3 mousePoint = new Vector3(eventData.position.x, eventData.position.y, zDistance);
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Touch ya Mouse drag ke sath X aur Y position update karna
        Vector3 mousePoint = new Vector3(eventData.position.x, eventData.position.y, zDistance);
        Vector3 targetPos = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
        
        transform.position = targetPos;

        // Check karein ki kya "beaker_glass" tag wale collider ke upar hai
        CheckBeakerCollision();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Jab drag chhodenge, toh original position aur rotation par wapas aa jayega
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    void CheckBeakerCollision()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && col.CompareTag("beaker_glass"))
            {
                Debug.Log("JAO HO DEBUG");
                break;
            }
        }
    }
}