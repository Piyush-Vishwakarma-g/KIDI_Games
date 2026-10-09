using UnityEngine;
using System.Collections;

public sealed class TrainGreatJobManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 1.0f; // Movement duration in seconds

    private void OnEnable()
    {
        StopDisableCoroutine();
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private void OnDisable()
    {
        StopDisableCoroutine();
    }

    private void StopDisableCoroutine()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(2f);

        TrainGameManager gameManager = TrainGameManager.instance;
        TrainLevelData levelData = TrainLevelData.instance;

        if (gameManager == null || levelData == null)
            yield break;

        int currentIndex = levelData.targetIndex;
        int maxIndex = gameManager.targetName.Length - 1;

        // Last target reached
        if (currentIndex >= maxIndex)
        {
            if (gameManager.levelUp != null)
               {
                    gameManager.levelUp.SetActive(true);
               } 

            gameObject.SetActive(false);
            yield break;
        }

        // Move to next target
        currentIndex++;
        levelData.targetIndex = currentIndex;

        gameManager.SetTargetName();
        gameManager.SetLevelObject();

        // ---------------- SMOOTH MOVEMENT LOGIC ----------------
        Transform targetTrain = (levelData.levelIndex == 1) 
            ? gameManager.TrainEngine.transform 
            : gameManager.MultiTrainEngine.transform;

        RectTransform rectTransform = targetTrain.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 targetPosition = startPosition;

        if (levelData.targetIndex == 1)
        {
            Debug.Log("I am here iii11  "+levelData.targetIndex);
            targetPosition = gameManager._targetPosition1;
        }
        else if (levelData.targetIndex == 3)
        {
            Debug.Log("I am here iii33  "+levelData.targetIndex);
            targetPosition = gameManager._targetPosition2;
        }
        

        if (targetPosition != startPosition)
        {
            // SAFE FIX: Coroutine ko TrainGameManager (Persistent Singleton) par start kar rahe hain
            // taaki GreatImage disable hone par bhi Coroutine stop na ho.
            gameManager.StartCoroutine(SmoothMoveUI(rectTransform, startPosition, targetPosition, moveDuration));
        }

        // GreatImage Object ko turant hide kar dein, movement background me GameManager dwara chalegi
      //  gameObject.SetActive(false);
    }

    private IEnumerator SmoothMoveUI(RectTransform rectTransform, Vector3 startPos, Vector3 targetPos, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (rectTransform == null) yield break;

            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // Smoothstep curve calculation

            rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (rectTransform != null)
        {
            rectTransform.localPosition = targetPos; // Final position safety assignment
        }
        gameObject.SetActive(false);
    }
}