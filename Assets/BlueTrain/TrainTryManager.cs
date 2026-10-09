using System.Collections;
using UnityEngine;

public class TrainTryManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
        int targetIdx = TrainLevelData.instance.targetIndex;
          if(TrainLevelData.instance.levelIndex==1)
          {
            TrainGameManager.instance.levelOneUI.SetActive(false); 
          } else
          {
                TrainGameManager.instance.leveltwoUIObject[targetIdx].SetActive(false); 
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
        int targetIdx = TrainLevelData.instance.targetIndex;
                        
        if(TrainLevelData.instance.levelIndex==1)
          {
            TrainGameManager.instance.levelOneUI.SetActive(true); 
          } else
          {
                TrainGameManager.instance.leveltwoUIObject[targetIdx].SetActive(true); 
          }  
    }
}