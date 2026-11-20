using System.Collections;
using UnityEngine;


public class AvatarGreatJobManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 private Coroutine disableCoroutine;
 public MobileTTS mobileTTS;

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
         mobileTTS.SpeakTxt("Bravo! Well done! You succeeded, try it without the tablet.");
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
