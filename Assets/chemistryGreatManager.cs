using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class chemistryGreatManager : MonoBehaviour
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
        
        
            gameObject.SetActive(false);
            SceneManager.LoadScene("Lab4");
            
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
