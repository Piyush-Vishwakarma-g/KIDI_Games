using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class HairStyle
{
    public GameObject[] hair;

}

public class AvatarGameManager : MonoBehaviour
{
    public static AvatarGameManager Instance { get; private set; }

    public GameObject chooseAvatarObj;
    public GameObject customAvatarObj;

    public GameObject targetEmotionObj;
    public GameObject targetShirtObject;

    public GameObject greatJobObj;
    public GameObject tryJobObj;
    public GameObject grilEmotionParent;
    public GameObject boyEmotionParent;

    [Header("Avatar Options")]
    public GameObject[] sexAvatar;
    public GameObject[] emotionAvation;
    public GameObject[] emotionBoyAvation;
    public GameObject[] hairColor;

    [Header("Selected Index")]
    public int sexIndex = 0;
    public int emotionIndex = 0;
    public int hairColorIndex = 0;

    public GameObject[] customSexAvatar;
    public GameObject targetTShirt;
    public GameObject girlHairStyleParent;
    public GameObject boyHairStyleParent;
    public List<HairStyle> hairStyles;
    public List<HairStyle> hairBoyStyles;
    public List<GameObject> eyesList;

    public List<GameObject> mouthList;
    public List<Color> colorList;

    public int hairIndex;
    public int eyeIndex;
    public int mouthIndex;
    public int shirtColorIndex;

    public GameObject boyHairParent;
    public GameObject girlHairParent;
    public Button[] hairBoyBtn;
    public Button[] hairBtn;
    public Button[] eyeBtn;
    public Button[] mouthBtn;
    public Button[] shirtBtn;

    public Sprite[] emotionSpriteAry;
    public Sprite[] emotionBoySpriteAry;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // SEX / BOY-GIRL
    // =========================

    public void SelectSex(int index)
    {
        if (index < 0 || index >= sexAvatar.Length)
            return;

        sexIndex = index;

        for (int i = 0; i < sexAvatar.Length; i++)
        {

            sexAvatar[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        sexAvatar[sexIndex].transform.GetChild(2).gameObject.SetActive(true);
        Debug.Log("Sex Selected: " + sexIndex);
        if(sexIndex==0)
        {
            boyEmotionParent.SetActive(true);
            grilEmotionParent.SetActive(false);
        }
        if(sexIndex==1)
        {
            boyEmotionParent.SetActive(false);
            grilEmotionParent.SetActive(true);
        }
    }

    // =========================
    // EMOTION
    // =========================

    public void SelectEmotion(int index)
    {
        if (index < 0 || index >= emotionAvation.Length)
            return;

        emotionIndex = index;

        Debug.Log("Emotion Selected: " + emotionIndex);
        for (int i = 0; i < emotionAvation.Length; i++)
        {
            emotionAvation[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < emotionBoyAvation.Length; i++)
        {
            emotionBoyAvation[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        if(sexIndex==0)
        {
            emotionBoyAvation[emotionIndex].transform.GetChild(0).gameObject.SetActive(true);
        }
        if(sexIndex==1)
        {
            emotionAvation[emotionIndex].transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    // =========================
    // HAIR COLOR
    // =========================

    public void SelectHairColor(int index)
    {
        if (index < 0 || index >= hairColor.Length)
            return;

        hairColorIndex = index;

        Debug.Log("Hair Color Selected: " + hairColorIndex);
        for (int i = 0; i < hairColor.Length; i++)
        {
            hairColor[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        hairColor[hairColorIndex].transform.GetChild(0).gameObject.SetActive(true);
    }

    // =========================
    // CONTINUE BUTTON
    // =========================

    public void OnContinueButton()
    {
        Debug.Log(
            "Avatar Selected -> " +
            "Sex: " + sexIndex +
            ", Emotion: " + emotionIndex +
            ", Hair: " + hairColorIndex
        );
        DisableALLBtn();
        ShowCustomAvatar();
        // Next scene
        // SceneManager.LoadScene("TrainGame");
    }
    public void ShowCustomAvatar()
    {
        OnHairClick(0);
        OnEyeClick(0);

        chooseAvatarObj.gameObject.SetActive(false);
        customAvatarObj.gameObject.SetActive(true);
        if (sexIndex == 0)
        {
            targetEmotionObj.GetComponent<Image>().sprite = emotionBoySpriteAry[emotionIndex];
            girlHairParent.SetActive(false);
            boyHairParent.SetActive(true);
            boyHairStyleParent.SetActive(true);
            girlHairStyleParent.SetActive(false);
        }
        else if (sexIndex == 1)
        {
            targetEmotionObj.GetComponent<Image>().sprite = emotionSpriteAry[emotionIndex];
            girlHairParent.SetActive(true);
            boyHairParent.SetActive(false);
            boyHairStyleParent.SetActive(false);
            girlHairStyleParent.SetActive(true);
        }

    }

    public void DisableAllHair()
    {
        if (hairStyles == null) return;

        for (int i = 0; i < hairStyles.Count; i++)
        {
            if (hairStyles[i] == null || hairStyles[i].hair == null) continue;

            for (int j = 0; j < hairStyles[i].hair.Length; j++) // Corrected: j++
            {
                if (hairStyles[i].hair[j] != null) // Corrected: hairStyles[i]
                {
                    hairStyles[i].hair[j].SetActive(false);
                }
            }
        }
        for (int i = 0; i < hairBoyStyles.Count; i++)
        {
            if (hairBoyStyles[i] == null || hairBoyStyles[i].hair == null) continue;

            for (int j = 0; j < hairBoyStyles[i].hair.Length; j++) // Corrected: j++
            {
                if (hairBoyStyles[i].hair[j] != null) // Corrected: hairStyles[i]
                {
                    hairBoyStyles[i].hair[j].SetActive(false);
                }
            }
        }
    }
    public void DisableEyeList()
    {
        if (eyesList == null) return;

        for (int i = 0; i < eyesList.Count; i++)
        {
            if (eyesList[i] == null) continue;

            eyesList[i].SetActive(false);

        }
    }
    public void DisableMouthList()
    {
        if (mouthList == null) return;

        for (int i = 0; i < mouthList.Count; i++)
        {
            if (mouthList[i] == null) continue;

            mouthList[i].SetActive(false);

        }
    }
    public void DisableALLBtn()
    {
        for (int i = 0; i < hairBtn.Length; i++)
        {
            hairBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < eyeBtn.Length; i++)
        {
            eyeBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < mouthBtn.Length; i++)
        {
            mouthBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        for (int i = 0; i < shirtBtn.Length; i++)
        {
            shirtBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }


    }
    public void OnHairClick(int val)
    {
        for (int i = 0; i < hairBtn.Length; i++)
        {
            hairBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < hairBoyBtn.Length; i++)
        {
            hairBoyBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        

        DisableAllHair();

        if (sexIndex == 0)
        {
            hairBoyStyles[hairColorIndex].hair[val].gameObject.SetActive(true);
            hairBoyBtn[val].transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (sexIndex == 1)
        {
            hairStyles[hairColorIndex].hair[val].gameObject.SetActive(true);
            hairBtn[val].transform.GetChild(0).gameObject.SetActive(true);
        }

    }
    public void OnEyeClick(int val)
    {
        for (int i = 0; i < eyeBtn.Length; i++)
        {
            eyeBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        eyeBtn[val].transform.GetChild(0).gameObject.SetActive(true);

        DisableEyeList();
        eyesList[val].SetActive(true);
    }
    public void OnMouthClick(int val)
    {
        for (int i = 0; i < mouthBtn.Length; i++)
        {
            mouthBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        mouthBtn[val].transform.GetChild(0).gameObject.SetActive(true);
        DisableMouthList();
        mouthList[val].SetActive(true);
    }
    public void OnShirtClick(int val)
    {
        for (int i = 0; i < shirtBtn.Length; i++)
        {
            shirtBtn[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        shirtBtn[val].transform.GetChild(0).gameObject.SetActive(true);

        targetShirtObject.GetComponent<Image>().color = colorList[val];
    }

    private bool HasActiveChild()
    {
        bool val1 = CheckArrayForActiveChild(eyeBtn);
        bool val2 = CheckArrayForActiveChild(mouthBtn);
        bool val3 = CheckArrayForActiveChild(shirtBtn);

        return val1 && val2 && val3;
    }

    // Parameter ko 'GameObject[]' se badal kar 'Button[]' kar diya hai
    private bool CheckArrayForActiveChild(Button[] btnArray)
    {
        if (btnArray == null) return false;

        for (int i = 0; i < btnArray.Length; i++)
        {
            if (btnArray[i] != null && btnArray[i].transform.childCount > 0)
            {
                // Button ki transform se pehle child ka activeSelf check ho raha hai
                if (btnArray[i].transform.GetChild(0).gameObject.activeSelf)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnDoneClick()
    {
        bool showResult = HasActiveChild();
        Debug.Log("showResult: " + showResult);

        if (showResult)
        {
            greatJobObj.SetActive(true);
            tryJobObj.SetActive(false);

        }
        else
        {
            greatJobObj.SetActive(false);
            tryJobObj.SetActive(true);
            Debug.Log("Koyi ek ya zyada category me active child missing hai!");
        }
    }
}