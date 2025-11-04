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

    public void LoadLevel(string sceneName)
    {
        // Opsiyonel: küçük bir ses efekti oynatabilirsin burada
        Debug.Log($"Loading level: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
