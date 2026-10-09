using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BagGreatJobManager : MonoBehaviour
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
        
         BagGameManager.instance.dummyObjects[BagLevelData.instance.targetIndex].SetActive(true);
        if (BagLevelData.instance.targetIndex < BagGameManager.instance.targetName.Length - 1)
        {
           BagLevelData.instance.targetIndex = BagLevelData.instance.targetIndex + 1;
            gameObject.SetActive(false);
            BagGameManager.instance.SetTargetName();
            BagGameManager.instance.SetLevelObject();
           
        }
        else
        {
            gameObject.SetActive(false);
            if (BagGameManager.instance.levelUp != null)
                BagGameManager.instance.levelUp.SetActive(true);
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