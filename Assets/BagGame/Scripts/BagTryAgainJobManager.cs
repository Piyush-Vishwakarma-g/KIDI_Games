using System.Collections;
using UnityEngine;

public class BagTryAgainJobManager : MonoBehaviour
{
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
        int targetIdx = BagLevelData.instance.targetIndex;
          if(BagLevelData.instance.levelIndex==0)
          {
            BagGameManager.instance.levelOneUI.SetActive(false); 
          } else
          {
                BagGameManager.instance.leveltwoUIObject[targetIdx].SetActive(false); 
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
        int targetIdx = BagLevelData.instance.targetIndex;
                        
        if(BagLevelData.instance.levelIndex==0)
          {
            BagGameManager.instance.levelOneUI.SetActive(true); 
          } else
          {
                BagGameManager.instance.leveltwoUIObject[targetIdx].SetActive(true); 
          }  
    }
}