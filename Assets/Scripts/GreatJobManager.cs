using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreatJobManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(2f);
        
        
        if (LevelData.instance.targetIndex < GameManager.instance.targetName.Length - 1)
        {
           LevelData.instance.targetIndex = LevelData.instance.targetIndex + 1;
            gameObject.SetActive(false);
            GameManager.instance.SetTargetName();
            GameManager.instance.SetLevelObject();
        }
        else
        {
            gameObject.SetActive(false);
            if (GameManager.instance.levelUp != null)
                GameManager.instance.levelUp.SetActive(true);
        }
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