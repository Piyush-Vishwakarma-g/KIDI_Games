using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using TMPro;

public class FinanceManager : MonoBehaviour
{
    public GameObject firstPanel;
    public GameObject secondPanel;  
    public int totalMaxSelectedItems=4;
    public int totalDepositMoneyValue=500;
    public Button backButton;
    public int[] fourSelectedItemIndexes = new int[4];
    public TMP_Text[] fourSelectedItemNames = new TMP_Text[4];
    public TMP_Text[] fourSelectedItemPrices = new TMP_Text[4];
    public Image[] fourSelectedItemImages = new Image[4];

    public TMP_Text totalPriceText;
    public TMP_Text totalPayOutText;

    public TMP_Text[] lastSelectedItemNames = new TMP_Text[4];
    public TMP_Text[] lastSelectedItemPrices = new TMP_Text[4];
    public Image[] lastSelectedItemImages = new Image[4];

    public TMP_Text lastTotalPriceText;
    
    
    public GameObject[] selectedItemsAry;
    public String[] selectedItemsNameAry;
    public String[] selectedItemsPriceAry;
    public Image[] selectedItemsSpriteAry;

    public TMP_Text lastSpeakingText;
    public Slider lastSlider;
    public TMP_Text lastSliderText;
    public TMP_Text totalDepositMoney;

    public GameObject greatJobPanel;

    public MobileTTS mobileTTS;
    
    

    public void CheckTotalSelectedItems()
    {
        int totalSelectedItems = 0;

        for (int i = 0; i < fourSelectedItemIndexes.Length; i++)
        {
            fourSelectedItemIndexes[i] = -1;
        }

        for (int i = 0; i < selectedItemsAry.Length; i++)
        {
            if (selectedItemsAry[i] != null && selectedItemsAry[i].activeSelf)
            {
                if (totalSelectedItems < fourSelectedItemIndexes.Length)
                {
                    fourSelectedItemIndexes[totalSelectedItems] = i;
                }

                totalSelectedItems++;
                if (totalSelectedItems == totalMaxSelectedItems)
                {
                    Debug.Log("You have exceeded the maximum number of selected items!");
                    secondPanel.SetActive(true);
                    firstPanel.SetActive(false);
                    mobileTTS.SpeakTxt("The  total  is "+ totalPayOutText.text + ",  can  you  pay  the  cashier?" );
        
                    break;
                }
            }
        }

        Debug.Log("Total Selected Items: " + totalSelectedItems);
        for (int i = 0; i < fourSelectedItemIndexes.Length; i++)
        {
            if (fourSelectedItemIndexes[i] != -1)
            {
                int index = fourSelectedItemIndexes[i];
                fourSelectedItemNames[i].text = selectedItemsNameAry[index];
                fourSelectedItemPrices[i].text = selectedItemsPriceAry[index];
                lastSelectedItemPrices[i].text = selectedItemsPriceAry[index];
                lastSelectedItemNames[i].text = selectedItemsNameAry[index];
                if (fourSelectedItemImages[i] == null)
                {
                    Debug.LogError($"Selected item image reference at slot {i} is not assigned.", this);
                    continue;
                }

                if (selectedItemsSpriteAry[index] == null || selectedItemsSpriteAry[index].sprite == null)
                {
                    Debug.LogError($"Source sprite for selected item at index {index} is not assigned.", this);
                    continue;
                }

                fourSelectedItemImages[i].sprite = selectedItemsSpriteAry[index].sprite;
                lastSelectedItemImages[i].sprite = selectedItemsSpriteAry[index].sprite;
            }
            else
            {
                fourSelectedItemNames[i].text = "";
                fourSelectedItemPrices[i].text = "";
                lastSelectedItemPrices[i].text = "";
                lastSelectedItemNames[i].text = "";
                if (fourSelectedItemImages[i] != null)
                {
                    fourSelectedItemImages[i].sprite = null;
                }
            }
        }
        totalPriceText.text = CalculateTotalPrice().ToString("F2");
        totalPayOutText.text = CalculateTotalPrice().ToString("F2");
        lastTotalPriceText.text = CalculateTotalPrice().ToString("F2");
        if (lastSlider != null)
        {
            lastSlider.maxValue = float.Parse(totalPayOutText.text);
        }
        totalDepositMoney.text = totalDepositMoneyValue.ToString("F2");
        lastSliderText.text = 0.ToString("F2");
        lastSlider.value = 0;
        lastSpeakingText.text = "0.00€";
    }

    private float CalculateTotalPrice()
    {
        float total = 0;
        for (int i = 0; i < fourSelectedItemIndexes.Length; i++)
        {
            if (fourSelectedItemIndexes[i] != -1)
            {
                float price;
                if (float.TryParse(selectedItemsPriceAry[fourSelectedItemIndexes[i]], out price))
                {
                    total += price;
                }
            }
        }
        return total;
    }

    public void OnLastSpeakBtn()
    {
         mobileTTS.SpeakTxt(lastSpeakingText.text);
    }

    public void OnEuroBtnClick(float value)
    {
        int payoutCents = Mathf.RoundToInt(lastSlider.maxValue * 100f);
        int currentCents = Mathf.RoundToInt(lastSlider.value * 100f);
        int selectedCents = Mathf.RoundToInt(value * 100f);
        int newTotalCents = currentCents + selectedCents;

        if (newTotalCents <= payoutCents)
        {
            lastSlider.value = newTotalCents / 100f;
            lastSliderText.text = lastSlider.value.ToString("F2");
            lastSpeakingText.text =  lastSlider.value.ToString("F2") + "€";
            totalDepositMoney.text = (totalDepositMoneyValue - lastSlider.value).ToString("F2");
            if (newTotalCents == payoutCents)
            {
                Debug.Log("You have selected the full payout amount.");
                lastSlider.fillRect.GetComponent<Image>().color = Color.green;
                greatJobPanel.SetActive(true);
                mobileTTS.SpeakTxt( "Well  done!  You  paid  the  exact  amount,  that's  great!");
            }
            else
            {
                lastSlider.fillRect.GetComponent<Image>().color = Color.orange;
            }
        }
        else
        {
            Debug.LogWarning("Selected value exceeds the total payout amount.");
        }
       
    }
    
}
