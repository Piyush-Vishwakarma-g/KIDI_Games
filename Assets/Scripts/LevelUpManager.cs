using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUpManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        LevelData.instance.levelIndex+=1;
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(2f);
        LevelData.instance.targetIndex=0;
        
        // Load the scene first (no need to deactivate the GameObject right before changing scenes)
        SceneManager.LoadScene("CubeGame");
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