using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUpManager : MonoBehaviour
{
    private Coroutine disableCoroutine;
    public MobileTTS mobileTTS;

    private void OnEnable()
    {
        //if(LevelData.instance.levelIndex<2)
        //{
           LevelData.instance.levelIndex+=1;
        //}
        
        GameManager.instance.PlaySuccessAudio();
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisTryAgainRoutine());
    }

    private IEnumerator DisTryAgainRoutine()
    {
        yield return new WaitForSeconds(1f);
        mobileTTS.SpeakTxt("You succeeded, try it without the tablet.");
        yield return new WaitForSeconds(4f);
        LevelData.instance.targetIndex=0;
        //this.gameObject.SetActive(false);
        //GameManager.instance.mainMeneu.SetActive(true);
        //ResetGameData();
        // Load the scene first (no need to deactivate the GameObject right before changing scenes)
        //SceneManager.LoadScene("CubeGame");
    }
    private void ResetGameData()
    {
        LevelData.instance.levelIndex = 0;
        LevelData.instance.targetIndex = 0;
        GameManager.instance.SetTargetName();
        for(int i = 0; i < GameManager.instance.levelCubeObj.Length; i++)
        {
            for(int j = 2; j < GameManager.instance.levelCubeObj[i].transform.childCount; j++)
            {
                GameManager.instance.levelCubeObj[i].transform.GetChild(j).GetChild(0).gameObject.SetActive(false);
        
            }
        }
        for(int i = 0; i < GameManager.instance.levelOneUI.transform.childCount; i++)
        {
            GameManager.instance.levelOneUI.transform.GetChild(i).gameObject.SetActive(true);
        }
        
        //GameManager.instance.levelCubeObj[LevelData.instance.levelIndex].transform.position=new Vector3(0,0,0);
        GameManager.instance.levelCubeObj[LevelData.instance.levelIndex].transform.rotation=Quaternion.Euler(0,0,0);
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