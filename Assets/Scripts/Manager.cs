using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public List<Extension> extensions;
    public GameObject goImage;
    public GameObject goTitle;
    public GameObject goCardHolder;
    public GameObject generalHolder;

    public List<DisplayCard> sortedCards;
    public StatsManager statsManager;

    public List<GameObject> titleTexts;
    public Button buttonStats;
    public TMP_Dropdown dropDownExtension;
    public Toggle toggleIsObtained;
    public ScrollRect scrollRect;
    public GameObject canvasCards;
    public GameObject canvasStats;

    [Serializable]
    public class ExtensionComposition
    {
        [HideInInspector] public string name;
        public List<BoostersComposition> boostersCompositions = new List<BoostersComposition>();
        public ExtensionComposition(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class BoostersComposition
    {
        [HideInInspector] public string name;
        public List<RarityPerBoosters> rarityPerBoosters = new List<RarityPerBoosters>();
        public BoostersComposition(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class RarityPerBoosters
    {
        public RarityManager.Rarity rarity;
        public int value;

        public RarityPerBoosters(RarityManager.Rarity rarity, int value)
        {
            this.rarity = rarity;
            this.value = value;
        }
    }

    public List<ExtensionComposition> extensionCompositions = new List<ExtensionComposition>();
    public Dictionary<string, ExtensionComposition> extensionCompositionsDict = new Dictionary<string, ExtensionComposition>();
    public StatsUI statsUI;

    void Start()
    {
        //dropdown.options.Add(new TMP_Dropdown.OptionData("All extensions"));
        //dropdown.onValueChanged.AddListener(DropdownOnValueChanged);
        buttonStats.onClick.AddListener(OnButtonStatsClicked);
        dropDownExtension.options.Add(new TMP_Dropdown.OptionData("Toutes les extensions"));
        toggleIsObtained.onValueChanged.AddListener(OnValueIsObtainedChanged);


        foreach (Extension extension in extensions)
        {
            extensionCompositions.Add(new ExtensionComposition(extension.name));
            if (extension.cartesCommunes.Count != 0)
            {
                extensionCompositions[extensionCompositions.Count - 1].boostersCompositions.Add(new BoostersComposition("Cartes communes"));

                foreach (RarityManager.Rarity rarity in Enum.GetValues(typeof(RarityManager.Rarity)))
                {
                    extensionCompositions[extensionCompositions.Count - 1].boostersCompositions[extensionCompositions[extensionCompositions.Count - 1].boostersCompositions.Count - 1].rarityPerBoosters.Add(new RarityPerBoosters(rarity, 0));
                }
            }

            foreach (Booster booster in extension.boosters)
            {
                extensionCompositions[extensionCompositions.Count - 1].boostersCompositions.Add(new BoostersComposition(booster.name));

                foreach (RarityManager.Rarity rarity in Enum.GetValues(typeof(RarityManager.Rarity)))
                {
                    extensionCompositions[extensionCompositions.Count - 1].boostersCompositions[extensionCompositions[extensionCompositions.Count - 1].boostersCompositions.Count - 1].rarityPerBoosters.Add(new RarityPerBoosters(rarity, 0));
                }
            }
            dropDownExtension.options.Add(new TMP_Dropdown.OptionData(extension.name));
            dropDownExtension.onValueChanged.AddListener(ChangeDisplayExtension);
        }
        extensionCompositionsDict = extensionCompositions.ToDictionary(ext => ext.name);
        SetupCards();
    }

    private void OnValueIsObtainedChanged(bool state)
    {
        ChangeDisplayExtension(dropDownExtension.value);
    }

    private void ChangeDisplayExtension(int index)
    {
        if (index == 0)
        {
            foreach (Transform child in generalHolder.transform)
            {
                child.gameObject.SetActive(true);
                if (child.childCount > 0 && child.GetComponentInChildren<DisplayCard>(true) != null)
                {
                    foreach (Transform carte in child)
                    {
                        if (toggleIsObtained.isOn)
                        {
                            carte.gameObject.SetActive(true);
                        }
                        else if (!toggleIsObtained.isOn && carte.GetComponent<DisplayCard>().isObtained)
                        {
                            carte.gameObject.SetActive(false);
                        }
                        else
                        {
                            carte.gameObject.SetActive(true);
                        }
                    }
                    if (child.TryGetComponent<GridLayoutGroup>(out var gridLayoutGroup))
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
                    }
                }
            }
        }
        else
        {
            foreach (Transform child in generalHolder.transform)
            {
                if (child.name == extensions[index - 1].name + " Title")
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.childCount > 0 && child.GetComponentInChildren<DisplayCard>(true) != null)
                {
                    DisplayCard displayCard = child.GetComponentInChildren<DisplayCard>(true);
                    if (displayCard.booster != null && displayCard.booster.extension.name == extensions[index - 1].name ||
                    displayCard.extension != null && displayCard.extension.name == extensions[index - 1].name)
                    {
                        child.gameObject.SetActive(true);
                        foreach (Transform carte in child)
                        {
                            if (toggleIsObtained.isOn)
                            {
                                carte.gameObject.SetActive(true);
                            }
                            else if (!toggleIsObtained.isOn && carte.GetComponent<DisplayCard>().isObtained)
                            {
                                carte.gameObject.SetActive(false);
                            }
                            else
                            {
                                carte.gameObject.SetActive(true);
                            }
                        }
                        if (child.TryGetComponent<GridLayoutGroup>(out var gridLayoutGroup))
                        {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
                        }
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                else if (child.gameObject.name != "Border")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void OnButtonStatsClicked()
    {
        canvasCards.SetActive(false);
        canvasStats.SetActive(true);

        if (statsUI.currentExtension != null)
        {
            var foundExtension = extensions.FirstOrDefault(ext => ext.name == statsUI.currentExtension.name);
            if (foundExtension != null)
            {
                statsManager.CalculStats(foundExtension, statsUI.secretsState);
                canvasStats.GetComponent<StatsUI>().toggleSecret.gameObject.SetActive(true);
            }
        }

    }

    void SetupCards()
    {
        foreach (Extension extension in extensions)
        {
            DisplayCards(extension);
        }

        GameObject newBorder = Instantiate(generalHolder.transform.GetChild(0).gameObject, generalHolder.transform);
        newBorder.name = "Border";
    }

    void DisplayCards(Extension extension)
    {
        GameObject newTitle = Instantiate(goTitle, generalHolder.transform);
        newTitle.name = extension.name + " Title";
        newTitle.GetComponent<Image>().sprite = extension.sprite;
        newTitle.GetComponent<TitreExtensionDatas>().extension = extension;

        DisplayExtensionDatas(extension, newTitle.transform);
        titleTexts.Add(newTitle);

        GameObject newImageHolder = Instantiate(goCardHolder, generalHolder.transform);

        foreach (Booster booster in extension.boosters)
        {
            booster.extension = extension;
            foreach (Carte carte in booster.cartes)
            {
                carte.booster = booster;
                SpawnCarte(carte, newImageHolder.transform);
            }
        }

        foreach (Carte carte in extension.cartesCommunes)
        {
            carte.extension = extension;
            SpawnCarte(carte, newImageHolder.transform);
        }

        DisplayCard[] cards = newImageHolder.GetComponentsInChildren<DisplayCard>();
        sortedCards = cards.OrderBy(c => c.id).ToList();

        for (int i = 0; i < sortedCards.Count; i++)
        {
            sortedCards[i].transform.SetSiblingIndex(i);

        }
    }

    void SpawnCarte(Carte carte, Transform holder)
    {
        GameObject newCard = Instantiate(goImage, holder);
        DisplayCard displayCard = newCard.GetComponent<DisplayCard>();
        displayCard.id = carte.id;
        newCard.name = carte.name;
        displayCard.sprite = carte.sprite;
        displayCard.rarity = carte.rarity;
        displayCard.booster = carte.booster;
        displayCard.extension = carte.extension;

        displayCard.isObtained = SaveDatas.Instance.Import(carte.id, carte.extension != null ? carte.extension.name : carte.booster.extension.name);
        if (carte.booster != null)
        {
            if ((int)carte.rarity < (int)RarityManager.Rarity.OneStar)
            {
                statsManager.numberOfCards[carte.booster.extension.name]++;
            }
            else
            {
                statsManager.numberOfSecretsCards[carte.booster.extension.name]++;
            }
        }
        else
        {
            if ((int)carte.rarity < (int)RarityManager.Rarity.OneStar)
            {
                statsManager.numberOfCards[carte.extension.name]++;
            }
            else
            {
                statsManager.numberOfSecretsCards[carte.extension.name]++;
            }
        }

        statsManager.AddCardToMissingList(displayCard);
        if (extensionCompositionsDict.TryGetValue(carte.extension != null ? carte.extension.name : carte.booster.extension.name, out var foundExtension))
        {
            int boosterIndex;
            if (carte.extension == null)
            {
                boosterIndex = foundExtension.boostersCompositions.FindIndex(boost => boost.name == carte.booster.name);
            }
            else
            {
                boosterIndex = 0;
            }
            foundExtension.boostersCompositions[boosterIndex].rarityPerBoosters[(int)carte.rarity].value++;
        }
    }

    public void DisplayExtensionDatas(Extension extension, Transform holder)
    {
        int totalCard = statsManager.GetNumberOfCards(extension);
        int totalMissingCards = statsManager.GetNumberOfMissingCards(extension);
    }
}