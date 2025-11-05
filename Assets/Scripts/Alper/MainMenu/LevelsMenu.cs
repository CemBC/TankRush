using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    [Header("Panel Reference")]
    public GameObject levelsMenu;

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
        LevelRuntimePasser.Current = data;
        SceneManager.LoadScene("GameScene");
    }
}
