using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Button backBtn;
    public GameObject mainMeneu;
    public GameObject levelOneUI;
    public GameObject[] leveltwoUIObject;
    public string[] targetName;
    public string[] targetColorName;

    public string targetTag;
    public TMP_Text title;
    public TMP_Text tasktitle;
    //public int targetIndex=0;
    //public int levelIndex=0;
    public TMP_Text levelText;
    private void Awake()
    {
        
       // Singleton pattern: Ensure only one instance exists across scenes
        
            instance = this;
           
        
    }

    [SerializeField] public BoxCollider cubeCollider;
    [SerializeField] public BoxCollider[] sidePhasesCollider;

    public GameObject greatJob;
    public GameObject tryAgainJob;
    public GameObject levelUp;
    [HideInInspector] public bool isDraggingUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // FIX 1: Proper Button OnClick listener assignment
        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        SetTargetName();

        if (LevelData.instance != null && levelText != null)
        {
            if(LevelData.instance.levelIndex>1)
            {
                mainMeneu.SetActive(false);
            }
            levelText.text = LevelData.instance.levelIndex.ToString();
            SetLevelObject();
            
        }

    }
    private void OnBackButtonClicked()
    {
        if (SceneManagerScript.Instance != null)
        {
            SceneManagerScript.Instance.BackToReactNative();
        }
        else
        {
            Debug.LogWarning("[GameManager] SceneManagerScript instance not found.");
        }
    }
    public void LoadGameLevel()
    {
        LevelData.instance.levelIndex=2;
        SetTargetName();

        if (LevelData.instance != null && levelText != null)
        {
            if(LevelData.instance.levelIndex>1)
            {
                mainMeneu.SetActive(false);
            }
            levelText.text = LevelData.instance.levelIndex.ToString();
            SetLevelObject();
            
        }
    }
    public void SetLevelObject()
    {
        switch (LevelData.instance.levelIndex)
            {
                case 1:
                    if (levelOneUI != null) levelOneUI.SetActive(true);
                    
                    if (leveltwoUIObject != null)
                    {
                        for (int i = 0; i < leveltwoUIObject.Length; i++)
                        {
                            if (leveltwoUIObject[i] != null)
                                leveltwoUIObject[i].SetActive(false);
                        }
                    }
                    tasktitle.text="";
                    tasktitle.text="Match the shape to its correct slot!";
                    break;

                default:
                    if (levelOneUI != null) levelOneUI.SetActive(false);

                    if (leveltwoUIObject != null)
                    {
                        for (int i = 0; i < leveltwoUIObject.Length; i++)
                        {
                            if (leveltwoUIObject[i] != null)
                                leveltwoUIObject[i].SetActive(false);
                        }

                        // Bounds check before accessing index
                        int targetIdx = LevelData.instance.targetIndex;
                        if (targetIdx >= 0 && targetIdx < leveltwoUIObject.Length && leveltwoUIObject[targetIdx] != null)
                        {
                            // FIX: Added missing semicolon at the end of this line
                            leveltwoUIObject[targetIdx].SetActive(true); 
                        }
                        tasktitle.text="";
                    tasktitle.text="Match the color and shape to its correct slot!";
                    }
                    break;
            }
    }
    public void SetTargetName()
    {
         if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        if (targetName != null && targetName.Length > 0 && LevelData.instance.targetIndex >= 0 && LevelData.instance.targetIndex < targetName.Length)
        {
            targetTag = targetName[LevelData.instance.targetIndex];
            Debug.Log("target name---" + targetTag);
            if(LevelData.instance.levelIndex==1)
            {
                     title.text = "Put " + targetTag + " into cube";
            }
            else{
                 title.text = "Put "+targetColorName[LevelData.instance.targetIndex]+" " + targetTag + " into cube";
            }
           
        }

        EndUiDrag();
        if (greatJob != null) greatJob.SetActive(false);
        if (tryAgainJob != null) tryAgainJob.SetActive(false);
    }

    public void EndUiDrag()
    {
        isDraggingUI = false;

        if (cubeCollider != null)
            cubeCollider.enabled = true;

        if (sidePhasesCollider == null) return;
        foreach (var sidePhase in sidePhasesCollider)
        {
            if (sidePhase != null)
                sidePhase.enabled = false;
        }
    }
}
