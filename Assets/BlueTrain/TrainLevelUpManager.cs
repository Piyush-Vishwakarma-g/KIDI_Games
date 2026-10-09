using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrainLevelUpManager : MonoBehaviour
{
    private Coroutine disableCoroutine;
[SerializeField] private float moveDuration = 1.0f; // Movement duration in seconds
    private void OnEnable()
    {
        TrainGameManager gameManager = TrainGameManager.instance;
        TrainLevelData levelData = TrainLevelData.instance;

         Transform targetTrain = (levelData.levelIndex == 1) 
            ? gameManager.TrainEngine.transform 
            : gameManager.MultiTrainEngine.transform;

        RectTransform rectTransform = targetTrain.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 targetPosition = startPosition;
        targetPosition = gameManager._targetPosition3;
         gameManager.StartCoroutine(SmoothMoveUI(rectTransform, startPosition, targetPosition, moveDuration));
        TrainLevelData.instance.levelIndex+=1;
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
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
        //gameObject.SetActive(false);
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(2f);
        TrainLevelData.instance.targetIndex=0;
        
        // Load the scene first (no need to deactivate the GameObject right before changing scenes)
        SceneManager.LoadScene("TrainGame");
    }

    private void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
    }
}
