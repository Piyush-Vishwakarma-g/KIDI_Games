using System.Collections;
using UnityEngine;

public class FinanceLevelUpManager : MonoBehaviour
{
   
    private Coroutine disableCoroutine;
    public MobileTTS mobileTTS;

    private void OnEnable()
    {
        //if(LevelData.instance.levelIndex<2)
        //{
           //LevelData.instance.levelIndex+=1;
        //}
        
       // GameManager.instance.PlaySuccessAudio();
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }   
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(4f);
        mobileTTS.SpeakTxt("Bravo! Well done You succeeded, try it without the tablet.");
        //yield return new WaitForSeconds(4f);
        //LevelData.instance.targetIndex=0;
        
       // GameManager.instance.mainMeneu.SetActive(true);
        // Load the scene first (no need to deactivate the GameObject right before changing scenes)
       // SceneManager.LoadScene("CubeGame");
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