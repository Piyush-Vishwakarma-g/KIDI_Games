using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinanceGreatJobManager : MonoBehaviour
{
   private Coroutine disableCoroutine;
 public GameObject levelUpPanel;
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
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        levelUpPanel.SetActive(true);
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