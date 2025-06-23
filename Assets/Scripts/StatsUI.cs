using System;
using System.Collections;
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
    public bool secretsState;
    public TMP_Dropdown dropdownExtension;

    public Extension currentExtension;

    [Serializable]
    public class SaveStats
    {
        public Booster booster;
        public Extension extension;
        //public List<float> listStats = new List<float>();
        public List<float> listPercent = new List<float>();
        public List<int> listFlat = new List<int>();
        public List<int> listTotal = new List<int>();
    }

    void Awake()
    {
        buttonChoseCards.onClick.AddListener(OnButtonChoseCardsClicked);
        toggleSecret.onValueChanged.AddListener(OnToggleSecretClicked);
        dropdownExtension.onValueChanged.AddListener(OnChangeExtension);
    }

    private void OnChangeExtension(int index)
    {
        StartCoroutine(HandleExtensionChange(index));
    }

    private IEnumerator HandleExtensionChange(int index)
    {
        foreach (Transform child in statsHolder.transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitForEndOfFrame(); // Attendre la fin du frame avant de continuer

        if (index == 0)
        {
            foreach (Extension extension in manager.extensions)
            {
                currentExtension = null;
                statsManager.CalculStats(extension, secretsState, false);
            }
        }
        else
        {
            currentExtension = manager.extensions[index - 1];
            statsManager.CalculStats(currentExtension, secretsState);
        }
    }

    void Start()
    {
        foreach (Extension extension in manager.extensions)
        {
            dropdownExtension.options.Add(new TMP_Dropdown.OptionData(extension.name));
            // GameObject newButton = Instantiate(buttonExtension, buttonHolder);
            // newButton.name = extension.name + " Button";
            // newButton.GetComponent<ButtonExtensionStats>().extension = extension;
            // newButton.GetComponent<Image>().sprite = extension.sprite;
        }
        OnChangeExtension(0);
    }

    public void DisplayStats(SaveStats saved, SaveStats savedTotal, bool details = true)
    {
        if (savedTotal.extension != null && savedTotal.extension.boosters.Count > 1)
        {
            GameObject newOthersStats;
            if (GameObject.Find(savedTotal.extension.name + " Total") == null)
            {
                newOthersStats = Instantiate(displayStat, statsHolder.transform);
                newOthersStats.name = savedTotal.extension.name + " Total";
                newOthersStats.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = savedTotal.extension.sprite;
            }
            else
            {
                newOthersStats = GameObject.Find(savedTotal.extension.name + " Total");
            }
            newOthersStats.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (savedTotal.listTotal[savedTotal.listTotal.Count - 1] - savedTotal.listFlat[savedTotal.listFlat.Count - 1]).ToString() + "/" + savedTotal.listTotal[savedTotal.listTotal.Count - 1].ToString();
            newOthersStats.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = "";

            for (int i = 0; i < savedTotal.listTotal.Count - 1; i++)
            {
                if (savedTotal.listTotal[i] > 0)
                {
                    newOthersStats.transform.GetChild(i + 1).gameObject.SetActive(true);
                    newOthersStats.transform.GetChild(i + 1).GetChild(1).GetComponent<TMP_Text>().text = (savedTotal.listTotal[i] - savedTotal.listFlat[i]).ToString() + "/" + savedTotal.listTotal[i].ToString();
                    newOthersStats.transform.GetChild(i + 1).GetChild(2).GetComponent<TMP_Text>().text = "";
                }
                else
                {
                    newOthersStats.transform.GetChild(i + 1).gameObject.SetActive(false);
                }
            }
        }
        if (details || savedTotal.extension.boosters.Count == 1)
        {
            GameObject newDisplayStats;
            if (GameObject.Find(saved.booster.name) == null)
            {
                newDisplayStats = Instantiate(displayStat, statsHolder.transform);
                newDisplayStats.name = saved.booster.name;
            }
            else
            {
                newDisplayStats = GameObject.Find(saved.booster.name);
            }
            newDisplayStats.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = saved.booster.sprite;
            newDisplayStats.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (saved.listTotal[saved.listTotal.Count - 1] - saved.listFlat[saved.listFlat.Count - 1]).ToString() + "/" + saved.listTotal[saved.listTotal.Count - 1].ToString();
            //newDisplayStats.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = ((float)Math.Round(saved.listPercent[saved.listPercent.Count - 1] * 100, 2)).ToString() + "%";
            newDisplayStats.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

            for (int i = 0; i < saved.listPercent.Count - 1; i++)
            {
                if (saved.listTotal[i] > 0)
                {
                    newDisplayStats.transform.GetChild(i + 1).gameObject.SetActive(true);
                    newDisplayStats.transform.GetChild(i + 1).GetChild(1).GetComponent<TMP_Text>().text = (saved.listTotal[i] - saved.listFlat[i]).ToString() + "/" + saved.listTotal[i].ToString();
                    //newDisplayStats.transform.GetChild(i + 1).GetChild(2).GetComponent<TMP_Text>().text = ((float)Math.Round(saved.listPercent[i] * 100, 2)).ToString() + "%";
                    newDisplayStats.transform.GetChild(i + 1).GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    newDisplayStats.transform.GetChild(i + 1).gameObject.SetActive(false);
                }
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
        secretsState = state;
        if (currentExtension != null)
        {
            statsManager.CalculStats(currentExtension, state);
        }
        else
        {
            foreach (Extension extension in manager.extensions)
            {
                statsManager.CalculStats(extension, state, false);
            }
        }
    }

}
