using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Button backBtn;
    public GameObject[] levelCubeObj;
    public GameObject mainMeneu;
    public GameObject levelOneUI;
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip BravoSound;
    [SerializeField] private AudioClip greatSound;
    [SerializeField] private AudioClip goodJobSound;
    [SerializeField] private AudioClip exelentSound;
    
    public GameObject[] leveltwoUIObject;
    public GameObject[] levelthreeUIObject;
    public GameObject[] levelfourUIObject;
    public GameObject[] levelfiveUIObject;

    public string[] targetName;
    public string[] targetColorName;

    public string targetTag;
    public TMP_Text title;
    public TMP_Text tasktitle;
    //public int targetIndex=0;
    //public int levelIndex=0;
    public TMP_Text levelText;
    
    [SerializeField] public BoxCollider cubeCollider;
    [SerializeField] public BoxCollider[] sidePhasesCollider;

    public GameObject greatJob;
    public GameObject tryAgainJob;
    public GameObject levelUp;
    [HideInInspector] public bool isDraggingUI;
    
    
    private void Awake()
    {

        // Singleton pattern: Ensure only one instance exists across scenes

        instance = this;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    public void Start()
    {
        DisableAllLevelCubeObjects();
        // FIX 1: Proper Button OnClick listener assignment
        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        SetTargetName();

        if (LevelData.instance != null && levelText != null)
        {
            if (LevelData.instance.levelIndex > 1)
            {
                mainMeneu.SetActive(false);
            }
            levelText.text = LevelData.instance.levelIndex.ToString();
            SetLevelObject();

        }
        levelCubeObj[LevelData.instance.levelIndex - 1].SetActive(true);

    }
    public void DisableAllLevelCubeObjects()
    {
        if (levelCubeObj != null)
        {
            foreach (var obj in levelCubeObj)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
    public void DisableAllLevelUIObjects()
    {
        levelOneUI.SetActive(false);
        if (leveltwoUIObject != null)
        {
            foreach (var obj in leveltwoUIObject)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
        if (levelthreeUIObject != null)
        {
            foreach (var obj in levelthreeUIObject)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
        if (levelfourUIObject != null)
        {
            foreach (var obj in levelfourUIObject)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
        if (levelfiveUIObject != null)
        {
            foreach (var obj in levelfiveUIObject)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
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
    public void LoadGameLevel(int val)
    {
        LevelData.instance.levelIndex = val;
        SetTargetName();

        if (LevelData.instance != null && levelText != null)
        {
            if (LevelData.instance.levelIndex > 1)
            {
                mainMeneu.SetActive(false);
            }
            levelText.text = LevelData.instance.levelIndex.ToString();
            SetLevelObject();

        }
        DisableAllLevelCubeObjects();
        levelCubeObj[LevelData.instance.levelIndex - 1].SetActive(true);
    }
    public void SetLevelObject()
    {
        switch (LevelData.instance.levelIndex)
        {
            case 1:
                DisableAllLevelUIObjects();
                if (levelOneUI != null) levelOneUI.SetActive(true);


                tasktitle.text = "";
                tasktitle.text = "Match the shape to its correct slot!";
                break;
            case 2:
                DisableAllLevelUIObjects();
                if (leveltwoUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = LevelData.instance.targetIndex;
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
                DisableAllLevelUIObjects();
                if (levelthreeUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = LevelData.instance.targetIndex;
                    if (targetIdx >= 0 && targetIdx < levelthreeUIObject.Length && levelthreeUIObject[targetIdx] != null)
                    {
                        // FIX: Added missing semicolon at the end of this line
                        levelthreeUIObject[targetIdx].SetActive(true);
                    }
                    tasktitle.text = "";
                    tasktitle.text = "Match the color and shape to its correct slot!";
                }
                break;
                break;
            case 4:
                DisableAllLevelUIObjects();
                if (levelfourUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = LevelData.instance.targetIndex;
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
                DisableAllLevelUIObjects();
                if (levelfiveUIObject != null)
                {
                    // Bounds check before accessing index
                    int targetIdx = LevelData.instance.targetIndex;
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
        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        if (targetName != null && targetName.Length > 0 && LevelData.instance.targetIndex >= 0 && LevelData.instance.targetIndex < targetName.Length)
        {
            targetTag = targetName[LevelData.instance.targetIndex];
            Debug.Log("target name---" + targetTag);
            if (LevelData.instance.levelIndex == 1)
            {
                title.text = "Put " + targetTag + " into cube";
            }
            else
            {
                title.text = "Put " + targetColorName[LevelData.instance.targetIndex] + " " + targetTag + " into cube";
            }

        }

        EndUiDrag();
        if (greatJob != null) greatJob.SetActive(false);
        if (tryAgainJob != null) tryAgainJob.SetActive(false);
    }


    public void EndUiDrag()
    {
        isDraggingUI = false;

        if (LevelData.instance != null && levelCubeObj != null)
        {
            int index = LevelData.instance.levelIndex - 1;

            if (index >= 0 && index < levelCubeObj.Length)
            {
                GameObject levelObject = levelCubeObj[index];

                if (levelObject != null)
                {
                    BoxCollider boxCollider =
                        levelObject.GetComponent<BoxCollider>();

                    if (boxCollider != null)
                    {
                        boxCollider.enabled = true;
                    }
                }
            }
        }

        if (sidePhasesCollider != null)
        {
            foreach (BoxCollider sidePhase in sidePhasesCollider)
            {
                if (sidePhase != null)
                {
                    sidePhase.enabled = false;
                }
            }
        }
    }

}
