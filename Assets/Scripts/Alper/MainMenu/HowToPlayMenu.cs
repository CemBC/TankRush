using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject howToPlayPanel; // HowToPlayMenu objesi

    void Awake()
    {
        // Oyuna girince kapalı başlasın (aktifse bile kapatır)
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
    }
}
