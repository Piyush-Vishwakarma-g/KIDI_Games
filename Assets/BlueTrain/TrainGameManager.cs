using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TrainGameManager : MonoBehaviour
{
    public static TrainGameManager instance;
    public Button backBtn;
     [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip BravoSound;
    [SerializeField] public AudioClip greatSound;
    [SerializeField] public AudioClip goodJobSound;
    [SerializeField] public AudioClip exelentSound;
    public Vector3 _targetPosition;
    public Vector3 _targetPosition1;
    public Vector3 _targetPosition2;
    public Vector3 _targetPosition3;
    public GameObject mainMeneu;
    public GameObject levelOneUI;
    public GameObject TrainEngine;
    public GameObject MultiTrainEngine;
    public GameObject MultiTrainEngine3;
    public GameObject MultiTrainEngine4;
    public GameObject MultiTrainEngine5;
    public float[] TrainEnginePosition;
    public GameObject[] leveltwoUIObject;
    public GameObject[] levelthreeUIObject;
    public GameObject[] levelfourUIObject;
    public GameObject[] levelfiveUIObject;
    public string[] targetName;
    public string[] targetlvl3Name;
    public string[] targetlvl4Name;
    public string[] targetlvl5Name;
    public string[] targetColorName;
    public string[] targetlvl3ColorName;
    public string[] targetlvl4ColorName;    
    public string[] targetlvl5ColorName;


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
        if (TrainLevelData.instance != null)
        {
            SetTrainEngineForLevel(TrainLevelData.instance.levelIndex);
        }
    }
    public void PlaySuccessAudio()
    {
        if (audioSource != null && BravoSound != null)
        {
            audioSource.PlayOneShot(BravoSound);
        }

    }
    public void PlayGoodJobAudio()
    {
        if (audioSource != null && goodJobSound != null)
        {
            audioSource.PlayOneShot(goodJobSound);
        }

    }
    public void PlayExcellentAudio()
    {
        if (audioSource != null && exelentSound != null)
        {
            audioSource.PlayOneShot(exelentSound);
        }

    }
    public void PlaygreatAudio()
    {
        if (audioSource != null && greatSound != null)
        {
            audioSource.PlayOneShot(greatSound);
        }

    }
    public void LoadGameLevel(int val)
    {
        TrainLevelData.instance.levelIndex = val;
        SetTargetName();

        if (TrainLevelData.instance != null)
        {
            if (TrainLevelData.instance.levelIndex > 1)
            {
                mainMeneu.SetActive(false);
            }
            SetTrainEngineForLevel(TrainLevelData.instance.levelIndex);
            levelText.text = TrainLevelData.instance.levelIndex.ToString();
            SetLevelObject();

        }
    }
    private void SetTrainEngineForLevel(int levelIndex)
    {
        if (TrainEngine != null) TrainEngine.SetActive(levelIndex == 1);
        if (MultiTrainEngine != null) MultiTrainEngine.SetActive(levelIndex == 2);
        if (MultiTrainEngine3 != null) MultiTrainEngine3.SetActive(levelIndex == 3);
        if (MultiTrainEngine4 != null) MultiTrainEngine4.SetActive(levelIndex == 4);
        if (MultiTrainEngine5 != null) MultiTrainEngine5.SetActive(levelIndex == 5);
    }
    public void DisableAllUIObjects()
    {
        if (levelOneUI != null) levelOneUI.SetActive(false);
        if (leveltwoUIObject != null)
        {
            for (int i = 0; i < leveltwoUIObject.Length; i++)
            {
                if (leveltwoUIObject[i] != null)
                    leveltwoUIObject[i].SetActive(false);
            }
        }
        if (levelthreeUIObject != null)
        {
            for (int i = 0; i < levelthreeUIObject.Length; i++)
            {
                if (levelthreeUIObject[i] != null)
                    levelthreeUIObject[i].SetActive(false);
            }
        }
        if (levelfourUIObject != null)
        {
            for (int i = 0; i < levelfourUIObject.Length; i++)
            {
                if (levelfourUIObject[i] != null)
                    levelfourUIObject[i].SetActive(false);
            }
        }
        if (levelfiveUIObject != null)
        {
            for (int i = 0; i < levelfiveUIObject.Length; i++)
            {
                if (levelfiveUIObject[i] != null)
                    levelfiveUIObject[i].SetActive(false);
            }
        }
    }
    public void SetLevelObject()
    {
        switch (TrainLevelData.instance.levelIndex)
        {
            case 1:
            DisableAllUIObjects();
                if (levelOneUI != null) levelOneUI.SetActive(true);

               
                tasktitle.text = "";
                tasktitle.text = "Match the shape to its correct slot!";
                break;

            case 2:
                DisableAllUIObjects();
                
                if (leveltwoUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = TrainLevelData.instance.targetIndex;
                    if (targetIdx >= 0 && targetIdx < leveltwoUIObject.Length && leveltwoUIObject[targetIdx] != null)
                    {
                        // FIX: Added missing semicolon at the end of this line
                        leveltwoUIObject[targetIdx].SetActive(true);
                    }
                    tasktitle.text = "";
                    tasktitle.text = "Match the color and shape to its correct slot!";
                }
                break;
            case 3:
                DisableAllUIObjects();
                
                if (levelthreeUIObject != null)
                {

                    // Bounds check before accessing index
                    int targetIdx = TrainLevelData.instance.targetIndex;
                    if (targetIdx >= 0 && targetIdx < levelthreeUIObject.Length && levelthreeUIObject[targetIdx] != null)
                    {
                        // FIX: Added missing semicolon at the end of this line
                        levelthreeUIObject[targetIdx].SetActive(true);
                    }
                    tasktitle.text = "";
                    tasktitle.text = "Match the color and shape to its correct slot!";
                }
                break;
            case 4:
                DisableAllUIObjects();
                
                if (levelfourUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = TrainLevelData.instance.targetIndex;
                    if (targetIdx >= 0 && targetIdx < levelfourUIObject.Length && levelfourUIObject[targetIdx] != null)
                    {
                        // FIX: Added missing semicolon at the end of this line
                        levelfourUIObject[targetIdx].SetActive(true);
                    }
                    tasktitle.text = "";
                    tasktitle.text = "Match the color and shape to its correct slot!";
                }
                break;
            case 5:
                DisableAllUIObjects();
                
                if (levelfiveUIObject != null)
                {
                    
                    // Bounds check before accessing index
                    int targetIdx = TrainLevelData.instance.targetIndex;
                    if (targetIdx >= 0 && targetIdx < levelfiveUIObject.Length && levelfiveUIObject[targetIdx] != null)
                    {
                        // FIX: Added missing semicolon at the end of this line
                        levelfiveUIObject[targetIdx].SetActive(true);
                    }
                    tasktitle.text = "";
                    tasktitle.text = "Match the color and shape to its correct slot!";
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
        ////if (targetName != null && targetName.Length > 0 && TrainLevelData.instance.targetIndex >= 0 && TrainLevelData.instance.targetIndex < targetName.Length)
        //{
            switch (TrainLevelData.instance.levelIndex)
            {
                case 1:
                    targetTag = targetName[TrainLevelData.instance.targetIndex];
                    Debug.Log("target name---" + targetTag);
                    title.text = "Fill the Wagon with the " + targetTag + ".";
                    break;
                case 2:
                    targetTag = targetName[TrainLevelData.instance.targetIndex];
                    Debug.Log("target name---" + targetTag);
                    title.text = "Fill the " + targetColorName[TrainLevelData.instance.targetIndex] + " " + targetTag + " !";
                    break;
                    case 3:
                    targetTag = targetlvl3Name[TrainLevelData.instance.targetIndex];
                    Debug.Log("target name---" + targetTag);
                    title.text = "Fill the " + targetlvl3ColorName[TrainLevelData.instance.targetIndex] + " " + targetTag + " !";
                    break;
                    case 4:
                    targetTag = targetlvl4Name[TrainLevelData.instance.targetIndex];
                    Debug.Log("target name---" + targetTag);
                    title.text = "Fill the " + targetlvl4ColorName[TrainLevelData.instance.targetIndex] + " " + targetTag + " !";
                    break;
                    case 5:
                    targetTag = targetlvl5Name[TrainLevelData.instance.targetIndex];
                    Debug.Log("target name---" + targetTag);
                    title.text = "Fill the " + targetlvl5ColorName[TrainLevelData.instance.targetIndex] + " " + targetTag + " !";
                    break;
                    
           // }
           

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
        TrainLevelData.instance.targetIndex = 0;
        TrainLevelData.instance.levelIndex = 1;
        SceneManager.LoadScene("TrainGame");
    }
}
