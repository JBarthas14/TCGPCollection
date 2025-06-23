using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class DisplayCard : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public Sprite sprite;
    public RarityManager.Rarity rarity;
    public bool isObtained;
    public Booster booster;
    public Extension extension;

    public StatsManager statsManager;
    Manager manager;

    public static event Action OnCardClicked;

    void Start()
    {
        statsManager = FindAnyObjectByType<StatsManager>();
        manager = FindAnyObjectByType<Manager>();
        GetComponent<Image>().sprite = sprite;
        if (booster != null)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = booster.sprite;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = extension.allBoostersSprite;
            if (extension.boosters.Count > 2)
            {
                transform.GetChild(0).transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.GetChild(0).transform.localScale = new Vector3(2, 1, 1);
            }
        }
        transform.GetChild(1).GetComponent<TMP_Text>().text = id.ToString();
        RefreshColor();
    }

    void RefreshColor()
    {
        if (!isObtained)
        {
            GetComponent<Image>().color = new Color32(123, 123, 123, 255);
        }
        else
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isObtained = !isObtained;
        statsManager.AddCardToMissingList(this);
        SaveDatas.Instance.SaveCard(this, isObtained);
        RefreshColor();
        if (!manager.toggleIsObtained.isOn && isObtained)
        {
            gameObject.SetActive(false);
        }

        OnCardClicked?.Invoke();

    }
}
