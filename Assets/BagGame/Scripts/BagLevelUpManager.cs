using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BagLevelUpManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (BagLevelData.instance != null)
        {
            BagLevelData.instance.levelIndex += 1;
        }

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private IEnumerator DisTryAgainRoutine()
    {
        // Using Realtime ensures the timer runs even if the game is paused (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(2f);
        
        if (BagLevelData.instance != null)
        {
            BagLevelData.instance.targetIndex = 0;
        }

        // Asynchronous loading prevents main thread frame stutter
        SceneManager.LoadSceneAsync("BagGame");
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