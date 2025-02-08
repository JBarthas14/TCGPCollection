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
        statsManager.CalculStats(extension);
        statsUI.toggleSecret.gameObject.SetActive(true);
    }
}
