using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsMenu : MonoBehaviour
{
    [Header("Panel Reference")]
    public GameObject levelsMenu;

    [Header("Levels")]
    public LevelData[] levels;

    [Header("Buttons Parent")]
    public Transform buttonsParent; 

    private Button[] levelButtons;

    const string LastUnlockedKey = "LastUnlockedLevelNumber";

    void Awake()
    {
        levelButtons = buttonsParent.GetComponentsInChildren<Button>();
        SetupButtons();
    }

    void SetupButtons()
    {
        int lastUnlocked = PlayerPrefs.GetInt(LastUnlockedKey, 1);

        for (int i = 0; i < levels.Length && i < levelButtons.Length; i++)
        {
            LevelData data = levels[i];
            Button btn = levelButtons[i];

            if (data == null || btn == null)
                continue;

            bool unlocked = data.levelNumber <= lastUnlocked;
            btn.interactable = unlocked;

            // OnClick'leri sıfırla ve yeniden ekle
            btn.onClick.RemoveAllListeners();

            if (unlocked)
            {
                LevelData captured = data;
                btn.onClick.AddListener(() =>
                {
                    LoadLevel(captured);
                });
            }
        }
    }

    public void OpenLevelsMenu()
    {
        levelsMenu.SetActive(true);
    }

    public void CloseLevelsMenu()
    {
        levelsMenu.SetActive(false);
    }

    public void LoadLevel(LevelData data)
    {
        Debug.Log("LoadLevel: " + data.levelNumber);
        LevelRuntimePasser.Current = data;
        SceneManager.LoadScene("GameScene");
    }


    public void OnPlayButtonClicked()
    {
        int lastUnlocked = PlayerPrefs.GetInt(LastUnlockedKey, 1);
        LevelData target = null;

        foreach (var data in levels)
        {
            if (data != null && data.levelNumber == lastUnlocked)
            {
                target = data;
                break;
            }
        }
        if (target == null)
        {
            foreach (var data in levels)
            {
                if (data != null && data.levelNumber == 1)
                {
                    target = data;
                    break;
                }
            }
        }
        if (target != null)
        {
            LoadLevel(target);
        }
        else
        {
            Debug.LogError("Hiç level bulunamadı, Levels arrayini kontrol et.");
        }
    }
}
