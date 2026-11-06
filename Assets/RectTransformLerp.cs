using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RectTransformLerp : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public RectTransform sourceRect;
    public RectTransform targetRect;

    public RectTransform[] tileRects; // Array of RectTransforms to animate

    public GameObject[] plantAry;

    public GameObject greatJobUI;

    public Button[] buttons; // Array of buttons to disable/enable

    [Header("Animation Settings")]
    public float duration = 0.5f; // Move hone mein kitna time lagega (seconds)

    [Tooltip("Check this if source and target have different parents/hierarchy")]
    public bool matchWorldSpace = false;
    public int startIndex = 4; // Starting index for the tileRects array

    void Start()
    {
        if (tileRects != null && startIndex >= 0 && startIndex < tileRects.Length)
        {
            sourceRect = tileRects[startIndex];
        }

        TriggerMove();
    }

    // Agar aap button click ya kisi event par chalana chahein toh yeh public method call karein
    public void TriggerMove()
    {
        if (sourceRect != null && targetRect != null)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothCopyRoutine(sourceRect, targetRect, duration, matchWorldSpace));
        }
    }

    public IEnumerator SmoothCopyRoutine(RectTransform source, RectTransform target, float animDuration, bool worldSpace)
    {
        float time = 0;

        if (worldSpace)
        {
            // --- WORLD SPACE LERP (Different Parents / Canvases) ---
            Vector3 startPos = target.position;

            while (time < animDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / animDuration);

                target.position = Vector3.Lerp(startPos, source.position, t);

                yield return null;
            }

            // Final snap
            target.position = source.position;
        }
        else
        {
            // --- LOCAL SPACE LERP (Same Parent / Canvas) ---
            Vector2 startAnchoredPos = target.anchoredPosition;

            while (time < animDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / animDuration);

                target.anchoredPosition = Vector2.Lerp(startAnchoredPos, source.anchoredPosition, t);

                yield return null;
            }

            // Final snap
            target.anchoredPosition = source.anchoredPosition;
        }
CheckRobotPositionAndSetButtons();
        Debug.Log("RectTransform Translation Complete!");
    }

    public void OnBottomButtonClick()
    {
        int value = startIndex;
        switch (value)
        {
            case 0:
                Debug.Log("Bottom button clicked at index 0");
                value = 1;
                break;
            case 1:
                Debug.Log("Bottom button clicked at index 1");
                break;
            case 2:
                Debug.Log("Bottom button clicked at index 2");
                value = 3;
                break;
            case 3:
            value = 4;
                Debug.Log("Bottom button clicked at index 3");

                break;
            case 4:
                Debug.Log("Bottom button clicked at index 4");
                
                break;
            case 5:
                Debug.Log("Bottom button clicked at index 5");
                 value = 6;
                break;
            case 6:
                Debug.Log("Bottom button clicked at index 6");
                 value = 7;
                break;
            case 7:
                Debug.Log("Bottom button clicked at index 7");
                break;
            case 8:
                Debug.Log("Bottom button clicked at index 1");
                 value = 9;
                break;
            case 9:
                Debug.Log("Bottom button clicked at index 2");
                 value = 10;
                break;
            case 10:
                Debug.Log("Bottom button clicked at index 3");
                break;
           
        }
        startIndex=value;
        sourceRect = tileRects[startIndex];
        Debug.Log("Bottom button clicked at index " + value);
       // EnableDisableButtons(false);
        TriggerMove();
    }
    public void OnUpButtonClick()
    {
         int value = startIndex;
        switch (value)
        {
            case 0:
                Debug.Log("Up button clicked at index 0");
                
                break;
            case 1:
                Debug.Log("Up button clicked at index 1");
                value = 0;
                break;
            case 2:
                Debug.Log("Up button clicked at index 2");
                
                break;
            case 3:
            value = 2;
                Debug.Log("Up button clicked at index 3");

                break;
            case 4:
                Debug.Log("Up button clicked at index 4");
                value = 3;
                break;
            case 5:
                Debug.Log("Up button clicked at index 5");
                 
                break;
            case 6:
                Debug.Log("Up button clicked at index 6");
                 value = 5;
                break;
            case 7:
                Debug.Log("Up button clicked at index 7");
                value = 6;
                break;
            case 8:
                Debug.Log("Up button clicked at index 1");
                 
                break;
            case 9:
                Debug.Log("Up button clicked at index 2");
                 value = 8;
                break;
            case 10:
                Debug.Log("Up button clicked at index 3");
                value = 9;
                break;
           
        }
        startIndex=value;
        sourceRect = tileRects[startIndex];
        Debug.Log("Up button clicked at index " + value);
        TriggerMove();
        //EnableDisableButtons(false);
    }
    public void OnLeftButtonClick()
    {
         int value = startIndex;
        switch (value)
        {
            case 0:
                Debug.Log("Left button clicked at index 0");
                break;
            case 1:
                Debug.Log("Left button clicked at index 1");
                break;
            case 2:
            value = 0;                
                Debug.Log("Left button clicked at index 2");                
                break;
            case 3:
            value = 1;
                Debug.Log("Left button clicked at index 3");

                break;
            case 4:
                Debug.Log("Left button clicked at index 4");                
                break;
            case 5:
                Debug.Log("Left button clicked at index 5");
                 value = 2;
                break;
            case 6:
                Debug.Log("Left button clicked at index 6");
                 value = 3;
                break;
            case 7:
                Debug.Log("Left button clicked at index 7");
                value = 4;
                break;
            case 8:
                Debug.Log("Left button clicked at index 1");
                 value = 5;
                break;
            case 9:
                Debug.Log("Left button clicked at index 2");
                 value = 6;
                break;
            case 10:
                value = 7;
                Debug.Log("Left button clicked at index 3");
                break;
           
        }
        startIndex=value;
        sourceRect = tileRects[startIndex];
        Debug.Log("Left button clicked at index " + value);
        TriggerMove();
        //EnableDisableButtons(false);
    }
    public void OnRightButtonClick()
    {
         int value = startIndex;
        switch (value)
        {
            case 0:
                Debug.Log("Right button clicked at index 0");
                value = 2;
                break;
            case 1:
                Debug.Log("Right button clicked at index 1");
                value = 3;
                break;
            case 2:
                Debug.Log("Right button clicked at index 2");
                value = 5;
                break;
            case 3:
            value = 6;
                Debug.Log("Right button clicked at index 3");

                break;
            case 4:
                Debug.Log("Right button clicked at index 4");
                 value = 7;
                break;
            case 5:
                Debug.Log("Right button clicked at index 5");
                 value = 8;
                break;
            case 6:
                Debug.Log("Right button clicked at index 6");
                 value = 9;
                break;
            case 7:
                Debug.Log("Right button clicked at index 7");
                 value = 10;
                break;
            case 8:
                Debug.Log("Right button clicked at index 1");
                // value = 9;
                break;
            case 9:
                Debug.Log("Right button clicked at index 2");
                 //value = 10;
                break;
            case 10:
                Debug.Log("Right button clicked at index 3");
                break;
           
        }
        startIndex=value;
        sourceRect = tileRects[startIndex];
        Debug.Log("Right button clicked at index " + value);
        TriggerMove();
        //EnableDisableButtons(false);
    }

    public void waterBtnClick()
    {
        Debug.Log("Water button clicked");
        // Add your water button logic here
        targetRect.transform.GetChild(0).gameObject.SetActive(true); // Activate the first child of targetRect
       EnableDisableButtons(false);
        StartCoroutine(DeactivateAfterDelay(targetRect.transform.GetChild(0).gameObject, 2f)); // Deactivate after 2 seconds    
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        plantAry[startIndex].SetActive(true); // Deactivate the corresponding plantAry element
        CheckAllFlowerActive();
        EnableDisableButtons(true);
        StopCoroutine(DeactivateAfterDelay(obj, delay)); // Stop the coroutine after deactivating
    }

    public void CheckAllFlowerActive()
    {
        bool allActive = true;
        foreach (GameObject plant in plantAry)
        {
            if (!plant.activeSelf)
            {
                allActive = false;
                break;
            }
        }

        if (allActive)
        {
            Debug.Log("All flowers are active!");
            greatJobUI.SetActive(true); // Activate the Great Job UI
        }
    }

    public void EnableDisableButtons(bool enable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = enable;
        }

        if (enable)
        {
            CheckRobotPositionAndSetButtons();
        }
    }

    private void CheckRobotPositionAndSetButtons()
    {
        if (buttons == null || buttons.Length < 4)
        {
            Debug.LogError("At least four buttons must be assigned.");
            return;
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true; // Enable all buttons initially
        }

        if (startIndex == 10 || startIndex == 9 || startIndex == 8)
        {
            buttons[3].interactable = false;
        }

        if (startIndex == 4 || startIndex == 1 || startIndex == 0)
        {
            buttons[2].interactable = false;
        }

        if (startIndex == 4 || startIndex == 7 || startIndex == 10)
        {
            buttons[0].interactable = false;
        }

        if (startIndex == 8 || startIndex == 5 || startIndex == 2 || startIndex == 0)
        {
            buttons[1].interactable = false;
        }
        if(startIndex==1 )
        {
            buttons[0].interactable = false;
        }
    }
}