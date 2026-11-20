using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TraverseLevelUpManager : MonoBehaviour
{
    
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
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        MazeTraverseManager.instance.mazeLevelIndex=0;
        MazeTraverseManager.instance.isblackBall = false;
        MazeTraverseManager.instance.isbedsheet = false;
        MazeTraverseManager.instance.isheadPhone = false;
        MazeTraverseManager.instance.istravel = false;
       // SceneManager.LoadScene("MazeTraverse");
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

