using UnityEngine;
using UnityEngine.EventSystems;

public class TrainSimpleUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        UnityEngine.Debug.Log("EndDrag: Event--" + eventData.position );
        try
        {
            if (TrainGameManager.instance != null && Physics.Raycast(ray, out RaycastHit hit))
            {
                UnityEngine.Debug.Log("EndDrag: Successfully dropped on target slot!" + hit.collider.name );
                    
                if (hit.collider.CompareTag(TrainGameManager.instance.targetTag) &&transform.name==TrainGameManager.instance.targetTag)
                {
                    UnityEngine.Debug.Log("EndDrag: Successfully dropped on target slot!" + transform.name + "--target name----" + TrainGameManager.instance.targetTag);
                    droppedOnCorrectSlot = true;

                    if (hit.collider.transform.childCount > 0)
                        hit.collider.transform.GetChild(0).gameObject.SetActive(true);

                    ShowGreatJob();
                    
                }
                else
                {
                    UnityEngine.Debug.Log("EndDrag: Dropped on wrong slot: " + hit.collider.name);
                    if (TrainGameManager.instance.greatJob != null) TrainGameManager.instance.greatJob.SetActive(false);
                    if (TrainGameManager.instance.tryAgainJob != null) TrainGameManager.instance.tryAgainJob.SetActive(true);
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
           if (TrainGameManager.instance == null) return;

        TrainGameManager.instance.isDraggingUI = true;

        

        if (TrainGameManager.instance.sidePhasesCollider != null)
        {
            foreach (var sidePhase in TrainGameManager.instance.sidePhasesCollider)
            {
                if (sidePhase != null) sidePhase.enabled = true;
            }
        }     
       
    }

    public void ShowGreatJob()
    { 
        if (TrainGameManager.instance == null) return;
        if (TrainGameManager.instance.greatJob != null) TrainGameManager.instance.greatJob.SetActive(true);
        if (TrainGameManager.instance.tryAgainJob != null) TrainGameManager.instance.tryAgainJob.SetActive(false);
    
       }

    private void ResetColliders()
    {
        if (TrainGameManager.instance != null)
            TrainGameManager.instance.EndUiDrag();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ShowGreatJob));
        ResetColliders();
    }
}