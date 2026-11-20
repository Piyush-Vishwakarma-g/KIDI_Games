using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotGreatJobManager : MonoBehaviour
{
    private Coroutine disableCoroutine;
    public GameObject levelUpPanel;
    public MobileTTS mobileTTS;

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
        mobileTTS.SpeakTxt("Bravo! Well done You succeeded, try it without the tablet.");
        gameObject.SetActive(false);
        levelUpPanel.SetActive(true);
       // SceneManager.LoadScene("Robot");
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