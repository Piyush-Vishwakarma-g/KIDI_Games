using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Lab4Manager : MonoBehaviour
{
    public static Lab4Manager instance;

    private bool addedSpeakButtonListener;

    public MobileTTS mobileTTS;
    
    public GameObject bakingSoda;
    public GameObject vinegar;

    public GameObject cubeDropAnimation;
    public GameObject beakerCubeLiquid;
     public GameObject flaskLiquid;
     public GameObject hotplateElementryFlask;


     public GameObject elementryFlask;
     public GameObject hotplate;

     public TouchDragLab4CubeWaypointMover cubeMover;
     public TouchDragLab4ElemetryFlask elemetryFlaskMover;
     public TouchDragBreakerCubeLiquid beakerMover;
     
    
    public GameObject targetObject;
    public GameObject titleObject;

    public GameObject labTitleObject;
    
    public Button continueButton;
    public GameObject modelstable;
    
    
    public GameObject btnUI;
    public TextMeshProUGUI speakBtnText;

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
        CheckAllUnableObjects();
        addedSpeakButtonListener = true;
        cubeMover.enabled = false;
        elemetryFlaskMover.enabled = false;
        beakerMover.enabled = false;
    }

 public void OnenableWaypointMover()
    {
        bakingSoda.GetComponent<TouchDragWaypointMover>().enabled = true ;
     }
    private void OnDestroy()
    {
       
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
        if (continueButton == null)
        {
            Debug.LogError("Continue button is not assigned; cannot update its interactable state.", this);
            return;
        }

        if (targetObject == null)
        {
            continueButton.interactable = false;
            Debug.LogError("Target object is not assigned; cannot check the lab items.", this);
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

            GameObject slotIndicator = slot.GetChild(0).gameObject;
            if (!slot.gameObject.activeSelf || !slotIndicator.activeSelf)
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
        labTitleObject.SetActive(true);
        targetObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        modelstable.SetActive(true);
        
        btnUI.SetActive(false);
        bakingSoda.GetComponent<TouchDragWaypointMover>().enabled = false ;
        vinegar.GetComponent<TouchDragVinegarWaypointMover>().enabled = false;
        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Transform slot = targetObject.transform.GetChild(i);
            slot.GetComponent<Collider>().enabled = false;
        }
        Debug.Log("Continue button clicked. Title hidden, target object shown, and model table activated."+titleObject.activeSelf+" "+targetObject.activeSelf+" "+modelstable.activeSelf);
    }
    // Update is called once per frame
    public void OnSpeakButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        
    }
    public void OnSpeak2ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

        
    }
    public void OnSpeak3ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

       
    }
    public void OnSpeak4ButtonClick()
    {
        if (mobileTTS == null)
        {
            Debug.LogError("MobileTTS is not assigned.", this);
            return;
        }

       
    }
}
