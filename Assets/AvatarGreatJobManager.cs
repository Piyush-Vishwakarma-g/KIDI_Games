using System.Collections;
using UnityEngine;


public class AvatarGreatJobManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 private Coroutine disableCoroutine;
 public MobileTTS mobileTTS;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
        
                
        mobileTTS.SpeakTxt("Bravo! Well done! You succeeded, try it without the tablet.");
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
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
