using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BagGameManager : MonoBehaviour
{
    public static BagGameManager instance;
    public Button backBtn;
    public GameObject levelOneUI;
    public GameObject[] leveltwoUIObject;
    public string[] targetName;
    public string[] targetColorName;
    public GameObject[] dummyObjects;

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

        if (BagLevelData.instance != null && levelText != null)
        {
            levelText.text = BagLevelData.instance.levelIndex.ToString();
            SetLevelObject();
            
        }
        for(int i=0;i<dummyObjects.Length;i++)
        {
            dummyObjects[i].SetActive(false);
        }
    }
    public void SetLevelObject()
    {
        switch (BagLevelData.instance.levelIndex)
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
                        int targetIdx = BagLevelData.instance.targetIndex;
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
        if (targetName != null && targetName.Length > 0 && BagLevelData.instance.targetIndex >= 0 && BagLevelData.instance.targetIndex < targetName.Length)
        {
            targetTag = targetName[BagLevelData.instance.targetIndex];
            Debug.Log("target name---" + targetTag+" target index---" + BagLevelData.instance.targetIndex);
            if(BagLevelData.instance.levelIndex==1)
            {
                     title.text = "Put " + targetTag + " into cube";
            }
            else{
                for(int i=0;i<sidePhasesCollider.Length;i++)
                {
                    sidePhasesCollider[i].gameObject.SetActive(false);
                }
                sidePhasesCollider[BagLevelData.instance.targetIndex].gameObject.SetActive(true);
                sidePhasesCollider[BagLevelData.instance.targetIndex].transform.GetChild(0).gameObject.SetActive(false);
                 title.text = "Can you put the <color="+targetColorName[BagLevelData.instance.targetIndex]+">"+targetColorName[BagLevelData.instance.targetIndex]+"</color> " + targetTag + " in my bag?";
                
            }
           
        }

        EndUiDrag();
        if (greatJob != null) greatJob.SetActive(false);
        if (tryAgainJob != null) tryAgainJob.SetActive(false);
    }

    public void EndUiDrag()
    {
        isDraggingUI = false;

        //if (cubeCollider != null)
          //  cubeCollider.enabled = true;

        if (sidePhasesCollider == null) return;
        foreach (var sidePhase in sidePhasesCollider)
        {
            if (sidePhase != null)
                sidePhase.enabled = false;
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
}
