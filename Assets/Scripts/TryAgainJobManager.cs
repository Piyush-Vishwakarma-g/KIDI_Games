using System.Collections;
using UnityEngine;

public class TryAgainJobManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
        int targetIdx = LevelData.instance.targetIndex;
          if(LevelData.instance.levelIndex==0)
          {
            GameManager.instance.levelOneUI.SetActive(false); 
          } else
          {
                GameManager.instance.leveltwoUIObject[targetIdx].SetActive(false); 
          }             
        
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
        int targetIdx = LevelData.instance.targetIndex;
                        
        if(LevelData.instance.levelIndex==0)
          {
            GameManager.instance.levelOneUI.SetActive(true); 
          } else
          {
                GameManager.instance.leveltwoUIObject[targetIdx].SetActive(true); 
          }  
    }
}