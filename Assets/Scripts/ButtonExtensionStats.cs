using UnityEngine;
using UnityEngine.UI;

public class ButtonExtensionStats : MonoBehaviour
{
    public Extension extension;
    StatsManager statsManager;
    StatsUI statsUI;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        statsManager = FindAnyObjectByType<StatsManager>();
        statsUI = FindAnyObjectByType<StatsUI>();
    }

    private void OnClick()
    {
        foreach (Transform child in statsUI.statsHolder.transform)
        {
            Destroy(child.gameObject);
        }
        statsManager.CalculStats(extension, statsUI.secretsState);
        statsUI.toggleSecret.gameObject.SetActive(true);
        statsUI.currentExtension = extension;
    }
}
