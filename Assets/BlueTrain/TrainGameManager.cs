using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TrainGameManager : MonoBehaviour
{
    public static TrainGameManager instance;
    public Button backBtn;
    public Vector3 _targetPosition;
    public Vector3 _targetPosition1;
    public Vector3 _targetPosition2;
    public Vector3 _targetPosition3;
    public GameObject mainMeneu;
    public GameObject levelOneUI;
    public GameObject TrainEngine;
    public GameObject MultiTrainEngine;
    public float[] TrainEnginePosition;
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

    //[SerializeField] public BoxCollider cubeCollider;
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

        if (TrainLevelData.instance != null && levelText != null)
        {
            levelText.text = TrainLevelData.instance.levelIndex.ToString();
            SetLevelObject();
            
        }
        if(TrainLevelData.instance.levelIndex>1)
        {
                TrainEngine.SetActive(false);
                MultiTrainEngine.SetActive(true);
        }else{
                TrainEngine.SetActive(true);
                MultiTrainEngine.SetActive(false);
        }
    }
    public void LoadGameLevel(int val)
    {
        TrainLevelData.instance.levelIndex=val;
        SetTargetName();

        if (TrainLevelData.instance != null )
        {
            if(TrainLevelData.instance.levelIndex>1)
            {
                mainMeneu.SetActive(false);
            }
            if(TrainLevelData.instance.levelIndex>1)
        {
                TrainEngine.SetActive(false);
                MultiTrainEngine.SetActive(true);
        }else{
                TrainEngine.SetActive(true);
                MultiTrainEngine.SetActive(false);
        }
            levelText.text = TrainLevelData.instance.levelIndex.ToString();
            SetLevelObject();
            
        }
    }
    public void SetLevelObject()
    {
        switch (TrainLevelData.instance.levelIndex)
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
                        int targetIdx =     TrainLevelData.instance.targetIndex;
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
        // FIX 1: Proper Button OnClick listener assignment
        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        if (targetName != null && targetName.Length > 0 && TrainLevelData.instance.targetIndex >= 0 && TrainLevelData.instance.targetIndex < targetName.Length)
        {
            targetTag = targetName[TrainLevelData.instance.targetIndex];
            Debug.Log("target name---" + targetTag);
            if(TrainLevelData.instance.levelIndex==1)
            {
                     title.text = "Fill the Wagon with the " + targetTag + ".";
            }
            else{
                 title.text = "Fill the "+targetColorName[TrainLevelData.instance.targetIndex]+" " + targetTag + " !";
            }
           
        }

        EndUiDrag();
        if (greatJob != null) greatJob.SetActive(false);
        if (tryAgainJob != null) tryAgainJob.SetActive(false);
    }

    public void EndUiDrag()
    {
        isDraggingUI = false;

        

        if (sidePhasesCollider == null) return;
        foreach (var sidePhase in sidePhasesCollider)
        {
            if (sidePhase != null)
                sidePhase.enabled = true;
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
    public void OnBackGameButtonClicked()
    {
        TrainLevelData.instance.targetIndex=0;
        TrainLevelData.instance.levelIndex=1;
        SceneManager.LoadScene("TrainGame");
    }
}
