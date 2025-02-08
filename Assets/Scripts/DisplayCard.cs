using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public Sprite sprite;
    public RarityManager.Rarity rarity;
    public bool isObtained;
    public Booster booster;
    public Extension extension;

    public StatsManager statsManager;


    void Start()
    {
        statsManager = FindAnyObjectByType<StatsManager>();
        GetComponent<Image>().sprite = sprite;
        if (booster != null)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = booster.sprite;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = extension.allBoostersSprite;
        }
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
        RefreshColor();
    }
}
