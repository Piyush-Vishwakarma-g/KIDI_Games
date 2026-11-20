using System.Collections;
using UnityEngine;


public class TraverseGreatJobManager : MonoBehaviour
{
    public MobileTTS mobileTTS;
    public GameObject levelUpPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        if(MazeTraverseManager.instance.mazeLevelIndex==3)
        {
            mobileTTS.SpeakTxt("Bravo! Well done! You succeeded, try it without the tablet.");
        
           levelUpPanel.SetActive(true);
        }
        
       // levelUpPanel.SetActive(true);
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
