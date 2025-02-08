using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Button buttonChoseCards;
    public GameObject canvasCards;
    public StatsManager statsManager;
    public Manager manager;

    public Transform buttonHolder;
    public GameObject buttonExtension;

    public GameObject statsHolder;
    public GameObject displayStat;

    public Toggle toggleSecret;

    [Serializable]
    public class SaveStats
    {
        public Booster booster;
        //public List<float> listStats = new List<float>();
        public List<float> listPercent = new List<float>();
        public List<float> listFlat = new List<float>();
        public List<float> listTotal = new List<float>();
    }

    void Awake()
    {
        buttonChoseCards.onClick.AddListener(OnButtonChoseCardsClicked);
        toggleSecret.onValueChanged.AddListener(OnToggleSecretClicked);
    }

    void Start()
    {
        foreach (Extension extension in manager.extensions)
        {
            GameObject newButton = Instantiate(buttonExtension, buttonHolder);
            newButton.name = extension.name + " Button";
            newButton.GetComponent<ButtonExtensionStats>().extension = extension;
            newButton.GetComponent<Image>().sprite = extension.sprite;
        }
    }

    public void DisplayStats(SaveStats saved, SaveStats savedOthers)
    {
        GameObject newOthersStats;
        if (GameObject.Find(savedOthers.booster.extension.name + " OthersCards") == null)
        {
            newOthersStats = Instantiate(displayStat, statsHolder.transform);
            newOthersStats.name = savedOthers.booster.extension.name + " OthersCards";
            newOthersStats.transform.GetChild(0).GetComponent<Image>().sprite = savedOthers.booster.extension.allBoostersSprite;
        }
        else
        {
            newOthersStats = GameObject.Find(savedOthers.booster.extension.name + " OthersCards");
        }
        newOthersStats.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = (savedOthers.listTotal[savedOthers.listTotal.Count - 1] - savedOthers.listFlat[savedOthers.listFlat.Count - 1]).ToString() + "/" + savedOthers.listTotal[savedOthers.listTotal.Count - 1].ToString();
        newOthersStats.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (savedOthers.listPercent[savedOthers.listPercent.Count - 1] * 100).ToString() + "%";

        for (int i = 0; i < savedOthers.listPercent.Count - 1; i++)
        {
            if (savedOthers.listTotal[i] > 0)
            {
                newOthersStats.transform.GetChild(i + 1).gameObject.SetActive(true);
                newOthersStats.transform.GetChild(i + 1).GetComponent<Image>().fillAmount = savedOthers.listPercent[i];
                newOthersStats.transform.GetChild(i + 1).GetChild(1).GetComponent<TMP_Text>().text = (savedOthers.listTotal[i] - savedOthers.listFlat[i]).ToString() + "/" + savedOthers.listTotal[i].ToString();
                newOthersStats.transform.GetChild(i + 1).GetChild(2).GetComponent<TMP_Text>().text = (savedOthers.listPercent[i] * 100).ToString() + "%";
            }
            else
            {
                newOthersStats.transform.GetChild(i + 1).gameObject.SetActive(false);
            }
        }
        GameObject newDisplayStats;
        if (GameObject.Find(saved.booster.name) == null)
        {
            newDisplayStats = Instantiate(displayStat, statsHolder.transform);
            newDisplayStats.name = saved.booster.name;
            newDisplayStats.transform.GetChild(0).GetComponent<Image>().sprite = saved.booster.sprite;
        }
        else
        {
            newDisplayStats = GameObject.Find(saved.booster.name);
        }

        newDisplayStats.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = (saved.listTotal[saved.listTotal.Count - 1] - saved.listFlat[saved.listFlat.Count - 1]).ToString() + "/" + saved.listTotal[saved.listTotal.Count - 1].ToString();
        newDisplayStats.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (saved.listPercent[saved.listPercent.Count - 1] * 100).ToString() + "%";

        for (int i = 0; i < saved.listPercent.Count - 1; i++)
        {
            if (saved.listTotal[i] > 0)
            {
                newDisplayStats.transform.GetChild(i + 1).gameObject.SetActive(true);
                newDisplayStats.transform.GetChild(i + 1).GetComponent<Image>().fillAmount = saved.listPercent[i];
                newDisplayStats.transform.GetChild(i + 1).GetChild(1).GetComponent<TMP_Text>().text = (saved.listTotal[i] - saved.listFlat[i]).ToString() + "/" + saved.listTotal[i].ToString();
                newDisplayStats.transform.GetChild(i + 1).GetChild(2).GetComponent<TMP_Text>().text = (saved.listPercent[i] * 100).ToString() + "%";
            }
            else
            {
                newDisplayStats.transform.GetChild(i + 1).gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonChoseCardsClicked()
    {
        canvasCards.SetActive(!canvasCards.activeInHierarchy);
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void OnToggleSecretClicked(bool state)
    {
        foreach (Extension extension in manager.extensions)
        {
            statsManager.CalculStats(extension, state);
        }
    }
}
