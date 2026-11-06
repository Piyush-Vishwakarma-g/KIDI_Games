using System;
using System.Collections;
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
    public TMP_Text totalChangeText;
    public GameObject changePannel;

    public GameObject greatJobPanel;

    public MobileTTS mobileTTS;
    private int totalTenderedCents;

    public void Start()
    {
        changePannel.SetActive(false);
        greatJobPanel.SetActive(false);
    }

    public void CheckTotalSelectedItems()
    {
        int totalSelectedItems = 0;
        bool shouldSpeakTotal = false;

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
                    Debug.Log("You have exceeded the maximum number of selected items!"+totalPriceText.text );
                    shouldSpeakTotal = !secondPanel.activeSelf;
                    secondPanel.SetActive(true);
                    firstPanel.SetActive(false);
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
        totalChangeText.text = "0.00";
        lastSliderText.text = 0.ToString("F2");
        lastSlider.value = 0;
        lastSpeakingText.text = "0.00€";
        totalTenderedCents = 0;

        if (shouldSpeakTotal)
        {
            StartCoroutine(SpeakTotalAfterDelay());
        }
    }

    private IEnumerator SpeakTotalAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (secondPanel.activeInHierarchy)
        {
            mobileTTS.SpeakTxt("The  total  is " + totalPriceText.text + ",  can  you  pay  the  cashier?");
        }
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
        int selectedCents = Mathf.RoundToInt(value * 100f);
        int previousTenderedCents = totalTenderedCents;
        totalTenderedCents += selectedCents;
        int paidCents = Mathf.Min(totalTenderedCents, payoutCents);
        int changeCents = Mathf.Max(0, totalTenderedCents - payoutCents);

        lastSlider.value = paidCents / 100f;
        lastSliderText.text = lastSlider.value.ToString("F2");
        lastSpeakingText.text = lastSlider.value.ToString("F2") + "€";
        totalDepositMoney.text = (totalDepositMoneyValue - lastSlider.value).ToString("F2");
        totalChangeText.text = (changeCents / 100f).ToString("F2");

        if (totalTenderedCents >= payoutCents)
        {
            lastSlider.fillRect.GetComponent<Image>().color = Color.green;
            //greatJobPanel.SetActive(true);
            StartCoroutine(ShowGreatJobPanelAfterDelay());

            if (previousTenderedCents < payoutCents)
            {
                if (changeCents == 0)
                {
                    Debug.Log("You have selected the full payout amount.");
                    mobileTTS.SpeakTxt("Well  done!  You  paid  the  exact  amount,  that's  great!");
                }
                else
                {
                    changePannel.SetActive(true);
                    Debug.Log("You have selected more than the payout amount. Your change is: " + totalChangeText.text);
                    mobileTTS.SpeakTxt("Well  done!  Your  change  is  " + totalChangeText.text + "  euros.");
                }
            }
        }
        else
        {
            lastSlider.fillRect.GetComponent<Image>().color = Color.orange;
        }
       
    }

    private IEnumerator ShowGreatJobPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        greatJobPanel.SetActive(true);
        StopCoroutine(ShowGreatJobPanelAfterDelay());
    }
}
