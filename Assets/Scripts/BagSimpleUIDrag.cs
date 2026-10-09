using UnityEngine;
using UnityEngine.EventSystems;

public class BagSimpleUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Camera mainCamera;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        mainCamera = Camera.main;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PrepareDropColliders();
    }

    public void OnDrag(PointerEventData eventData)
    {
        PrepareDropColliders();

        float scale = (parentCanvas != null) ? parentCanvas.scaleFactor : 1f;
        rectTransform.anchoredPosition += eventData.delta / scale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        bool droppedOnCorrectSlot = false;

        try
        {
            if (BagGameManager.instance != null && Physics.Raycast(ray, out RaycastHit hit))
            {
                UnityEngine.Debug.Log("Successfully dropped on target slot!" + hit.collider.name );
                    
                if (hit.collider.CompareTag(BagGameManager.instance.targetTag) &&transform.name==BagGameManager.instance.targetTag)
                {
                    UnityEngine.Debug.Log("Successfully dropped on target slot!" + transform.name + "--target name----" + BagGameManager.instance.targetTag);
                    droppedOnCorrectSlot = true;

                    if (hit.collider.transform.childCount > 0)
                        hit.collider.transform.GetChild(0).gameObject.SetActive(true);

                    ShowGreatJob();
                    
                }
                else
                {
                    UnityEngine.Debug.Log("Dropped on wrong slot: " + hit.collider.name);
                    if (BagGameManager.instance.greatJob != null) BagGameManager.instance.greatJob.SetActive(false);
                    if (BagGameManager.instance.tryAgainJob != null) BagGameManager.instance.tryAgainJob.SetActive(true);
                }
            }
        }
        finally
        {
            ResetColliders();
        }

        if (droppedOnCorrectSlot)
            gameObject.SetActive(false);
        else
            rectTransform.anchoredPosition = startPosition;
    }

    private void PrepareDropColliders()
    {
        UnityEngine.Debug.Log("Successfully PrepareDropColliders on target slot!"  );
           if (BagGameManager.instance == null) return;

        BagGameManager.instance.isDraggingUI = true;

        

        if (BagGameManager.instance.sidePhasesCollider != null)
        {
            foreach (var sidePhase in BagGameManager.instance.sidePhasesCollider)
            {
                if (sidePhase != null) sidePhase.enabled = true;
            }
        }     
       
    }

    public void ShowGreatJob()
    { 
        if (BagGameManager.instance == null) return;
        if (BagGameManager.instance.greatJob != null) BagGameManager.instance.greatJob.SetActive(true);
        if (BagGameManager.instance.tryAgainJob != null) BagGameManager.instance.tryAgainJob.SetActive(false);
    
       }

    private void ResetColliders()
    {
        if (BagGameManager.instance != null)
            BagGameManager.instance.EndUiDrag();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ShowGreatJob));
        ResetColliders();
    }
}