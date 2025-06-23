using TMPro;
using UnityEngine;

public class TitreExtensionDatas : MonoBehaviour
{
    public Extension extension;
    public TMP_Text textAllCards;
    public TMP_Text textSecrets;
    StatsManager statsManager;

    void Start()
    {
        statsManager = FindAnyObjectByType<StatsManager>();
        DisplayCard.OnCardClicked += UpdateTexts;
        UpdateTexts();
    }

    void UpdateTexts()
    {
        int totalCard = statsManager.GetNumberOfCards(extension);
        int totalMissing = statsManager.GetNumberOfMissingCards(extension);

        int totalSecret = statsManager.GetNumberOfSecrets(extension);
        int totalSecretMissing = statsManager.GetNumberOfMissingSecrets(extension);

        textAllCards.text = (totalCard - totalMissing).ToString() + "/" + totalCard.ToString();
        textSecrets.text = (totalSecret - totalSecretMissing).ToString() + "/" + totalSecret.ToString();

    }
}
