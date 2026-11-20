using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BagLevelUpManager : MonoBehaviour
{
    private Coroutine disableCoroutine;
    public BagMobileTTS mobileTTS;

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
        yield return new WaitForSeconds(1f);
        mobileTTS.SpeakTxt("Bravo! You succeeded, try it without the tablet.");
        
        // Using Realtime ensures the timer runs even if the game is paused (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(4f);
        
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