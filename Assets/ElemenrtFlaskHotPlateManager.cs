using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElemenrtFlaskHotPlateManager : MonoBehaviour
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
        
        
            Lab4Manager.instance.greatObj.SetActive(true);
          //  SceneManager.LoadScene("Lab4");
            
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
