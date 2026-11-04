using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChemistryLab1Manager : MonoBehaviour
{
    public static ChemistryLab1Manager instance;

    private bool addedSpeakButtonListener;

    public MobileTTS mobileTTS;
    public TMP_Text speakingText;
    public TMP_Text speaking2Text;
    public TMP_Text speaking3Text;
    public TMP_Text speaking4Text;
    public Button speakButton;
    public Button speakButton2;
    public Button speakButton3;
    public Button speakButton4;
    public GameObject bakingSoda;
    public GameObject vinegar;
    
    public GameObject targetObject;
    public GameObject titleObject;
    public Button continueButton;
    public GameObject modelstable;
    public GameObject modelUI;
    public GameObject modelUI2;
    public GameObject modelUI3;
    public GameObject modelUI4;
    
    public GameObject btnUI;

    public GameObject worldUI;

    public GameObject greatObj;
    public GameObject tryObj;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        continueButton.interactable = false;
        modelUI.SetActive(false);
        modelUI2.SetActive(false);
        modelUI3.SetActive(false);

        if (speakButton == null)
        {
            Debug.LogError("Speak Button is not assigned.", this);
            return;
        }

        for (int i = 0; i < speakButton.onClick.GetPersistentEventCount(); i++)
        {
            if (speakButton.onClick.GetPersistentTarget(i) == this &&
                speakButton.onClick.GetPersistentMethodName(i) == nameof(OnSpeakButtonClick))
            {
                return;
            }
        }

        speakButton.onClick.AddListener(OnSpeakButtonClick);
        speakButton2.onClick.AddListener(OnSpeak2ButtonClick);
        speakButton3.onClick.AddListener(OnSpeak3ButtonClick);
        speakButton4.onClick.AddListener(OnSpeak4ButtonClick);

        addedSpeakButtonListener = true;
    }

 public void OnenableWaypointMover()
    {
        bakingSoda.GetComponent<TouchDragWaypointMover>().enabled = true ;
     }
    private void OnDestroy()
    {
        if (addedSpeakButtonListener && speakButton != null)
        {
            speakButton.onClick.RemoveListener(OnSpeakButtonClick);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnBtnClick()
    {
        //bool allChildrenActive = true;
        // for (int i = 0; i < targetObject.transform.childCount; i++)
        // {
        //     Transform child = targetObject.transform.GetChild(i);

        //     if (!child.gameObject.activeSelf)
        //     {
        //         allChildrenActive = false;
        //         break;
        //     }
        // }

        // continueButton.interactable = allChildrenActive;
    }
    public void CheckAllUnableObjects()
    {
        Debug.LogError("Target objec333333t is not assigned; cannot check the lab items.", this);
           
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned; cannot check the lab items.", this);
            return;
        }

        if (continueButton == null)
        {
            Debug.LogError("Continue button is not assigned; cannot update its interactable state.", this);
            return;
        }

        bool allChildrenActive = targetObject.transform.childCount > 0;
        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Transform slot = targetObject.transform.GetChild(i);
            if (slot.childCount == 0)
            {
                Debug.LogError($"Target slot '{slot.name}' has no item indicator child.", slot);
                allChildrenActive = false;
                break;
            }

            if (!slot.GetChild(0).gameObject.activeSelf)
            {
                allChildrenActive = false;
                break;
            }
        }

        continueButton.interactable = allChildrenActive;
        Debug.Log(allChildrenActive
            ? "All target items are active. Continue button is now interactable."
            : "Some target items are still missing. Continue button remains disabled.");
    }

    public void OnContinueClick()
    {
        titleObject.SetActive(false);
        targetObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        modelstable.SetActive(true);
        modelUI.SetActive(true);
        btnUI.SetActive(false);
        bakingSoda.GetComponent<TouchDragWaypointMover>().enabled = false ;
        vinegar.GetComponent<TouchDragVinegarWaypointMover>().enabled = false;
        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Transform slot = targetObject.transform.GetChild(i);
            slot.GetComponent<Collider>().enabled = false;
        }
    }
    // Update is called once per frame
    public void OnSpeakButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        if (speakingText == null || string.IsNullOrWhiteSpace(speakingText.text))
        {
            Debug.LogWarning("There is no text to speak.", this);
            return;
        }
        Debug.Log("Speaking text: " + speakingText.text);
        mobileTTS.SpeakTxt(speakingText.text);
    }
    public void OnSpeak2ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        if (speaking2Text == null || string.IsNullOrWhiteSpace(speaking2Text.text))
        {
            Debug.LogWarning("There is no text to speak.", this);
            return;
        }
        Debug.Log("Speaking2 text: " + speaking2Text.text);
        mobileTTS.SpeakTxt(speaking2Text.text);
    }
    public void OnSpeak3ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        if (speaking3Text == null || string.IsNullOrWhiteSpace(speaking3Text.text))
        {
            Debug.LogWarning("There is no text to speak.", this);
            return;
        }
        Debug.Log("Speaking 3text: " + speaking3Text.text);
        mobileTTS.SpeakTxt(speaking3Text.text);
    }
    public void OnSpeak4ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        if (speaking4Text == null || string.IsNullOrWhiteSpace(speaking4Text.text))
        {
            Debug.LogWarning("There is no text to speak.", this);
            return;
        }
        Debug.Log("Speaking 4text: " + speaking4Text.text);
        mobileTTS.SpeakTxt(speaking4Text.text);
    }
}
