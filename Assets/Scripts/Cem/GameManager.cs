using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject blackBackgroundObject;
    public GameObject nextLevelButton;
    private Image blackBackground;
    public GameObject LosePopup;
    public GameObject WinPopup;
    public static GameManager Instance { get; private set; }
    private bool IsWaveActive;
    public List<GameObject> unitButtons; 
    public RectTransform envanterBar;
    public RectTransform whiteArrow;
    private bool isHidden = false;
    private Vector2 shownPos;
    private Vector2 hiddenPos;
    public LevelData currentLevel;
    public int maxUnits = 5;
    private int currentUnits = 0;
    
    private int money;
    private int health;
    private Color defaultMoneyColor;
    private Color defaultUnitColor;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text healthText;

    [SerializeField] private TMP_Text levelMaxUnitText;

    [SerializeField] private TMP_Text currentUnitText;

    const string LastUnlockedKey = "LastUnlockedLevelNumber";


    void Awake()
    {
        Time.timeScale = 1f;  //Oyun bitince sıfır yapmıştık direkt burada sağlamayı 1 yaparak yeni seviyede sorun çıkmasın
        blackBackground = blackBackgroundObject.GetComponent<Image>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentLevel = LevelRuntimePasser.Current;
    }
    void Start()
    {
        shownPos = envanterBar.anchoredPosition;
        hiddenPos = shownPos + new Vector2(envanterBar.rect.width-5, 0);

        if (currentLevel != null)
        {
            maxUnits = currentLevel.maxUnits;
            money = currentLevel.levelStartupMoney;
            health = currentLevel.levelHealth;
        }
        if(levelMaxUnitText != null)
        {
            levelMaxUnitText.text = currentLevel.maxUnits.ToString();
        }
        if (moneyText != null)
        {
            defaultMoneyColor = moneyText.color;
        }
        if (currentUnitText != null)
        {
            defaultUnitColor = currentUnitText.color;
        }

        ApplyUnitAvailability();
        UpdateHealthUI();
        UpdateMoneyUI();
        Debug.Log("başlangıç paran:" + money);
        Debug.Log("başlangıç canın:" + health);
    }

    #region Placement Check
    public bool CanPlaceUnit()
    {
        return currentUnits < maxUnits;
    }
    public void AddUnit()
    {
        currentUnits = Mathf.Min(currentUnits + 1, maxUnits);
        UpdateUnitUI();
        Debug.Log("Current Unit:" + currentUnits);
    }

    public void RemoveUnit()
    {
        currentUnits = Mathf.Max(currentUnits - 1, 0);
        UpdateUnitUI();
        Debug.Log("Current Unit:" + currentUnits);
    }

    public void UpdateUnitUI()
    {
        if (currentUnitText != null)
            currentUnitText.text = currentUnits.ToString();
    }

    public void MaxUnitFeedBack()
    {
        if (currentUnitText == null || levelMaxUnitText == null) return;

        float duration = 0.15f;
        currentUnitText.DOKill();
        levelMaxUnitText.DOKill();
        currentUnitText.transform.DOScale(1.25f, duration);
        levelMaxUnitText.transform.DOScale(1.25f, duration);
        currentUnitText.DOColor(Color.red, duration);
        levelMaxUnitText.DOColor(Color.red, duration)
        .OnComplete(() =>
            {
                currentUnitText.transform.DOScale(1f, duration);
                levelMaxUnitText.transform.DOScale(1f, duration);
                currentUnitText.DOColor(defaultUnitColor, duration);
                levelMaxUnitText.DOColor(defaultUnitColor, duration);
            });
    }

    #endregion


    #region Money
    public bool TrySpend(int amount)
    {
        if (money < amount) return false;

        money -= amount;
        UpdateMoneyUI();
        Debug.Log("Yeni paran harcadıktan sonra :" + money);
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        money += amount;
        UpdateMoneyUI();
        Debug.Log("Yeni paran eklendikten sonra:" + money);
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }

    public void NoMoneyFeedback()
    {
        if (moneyText == null) return;
        float duration = 0.15f;
        moneyText.transform.DOScale(1.25f, duration);
        moneyText.DOColor(Color.red, duration)
        .OnComplete(() =>
        {
            moneyText.transform.DOScale(1f, duration);
            moneyText.DOColor(defaultMoneyColor, duration);
        });
    }

    #endregion

    #region Health
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Time.timeScale = 0f;
            blackBackgroundObject.SetActive(true);
            LosePopup.SetActive(true);
            blackBackground.DOFade(0.8f, 0.3f).SetEase(Ease.Linear).SetUpdate(true);
            LosePopup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            LosePopup.SetActive(true);
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = health.ToString();
    }

    public int getHealth() {return health;}
    #endregion

    public void ToggleEnvanterBar()
    {
        envanterBar.DOKill();
        if (isHidden)
        {
            envanterBar.DOAnchorPos(shownPos, 0.35f).SetEase(Ease.OutBack);
            whiteArrow.localEulerAngles = new Vector3(0f, 0f, -90);
            isHidden = false;
        }
        else
        {
            envanterBar.DOAnchorPos(hiddenPos, 0.35f).SetEase(Ease.InBack).OnComplete(() =>
            {
                whiteArrow.localEulerAngles = new Vector3(0f, 0f, 90);
            });
            isHidden = true;
        }
    }

    private void ApplyUnitAvailability()
    {
        if (unitButtons == null || unitButtons.Count == 0)
        {
            return;
        }
        for (int i = 0; i < unitButtons.Count; i++)
        {
            bool isActive = i < currentLevel.unitAvailability.Count ? currentLevel.unitAvailability[i] : false;
            unitButtons[i].SetActive(isActive);
        }
    }

    public void SetWaveActive(bool active)
    {
        IsWaveActive = active;
    }
    
    public bool getWaveInfo()
    {
        return IsWaveActive;
    }

    public void onWin()
    {
        UnlockNextLevel();
        Time.timeScale = 0f;

        blackBackgroundObject.SetActive(true);
        WinPopup.SetActive(true);
        blackBackground.DOFade(0.8f, 0.3f).SetEase(Ease.Linear).SetUpdate(true);
        WinPopup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);

        LevelData current = LevelRuntimePasser.Current;
        if(current.nextLevel == null)
        {
            nextLevelButton.SetActive(false);
        }
    }

    void UnlockNextLevel()
    {
        LevelData current = LevelRuntimePasser.Current;
        if (current == null) return;
        if (current.nextLevel == null) return;
        int lastUnlocked = PlayerPrefs.GetInt(LastUnlockedKey, 1);
        int nextNumber   = current.nextLevel.levelNumber;
        if (nextNumber > lastUnlocked)
        {
            PlayerPrefs.SetInt(LastUnlockedKey, nextNumber);
            PlayerPrefs.Save();
        }
    }

    public void onRetryButtonClicked()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("GameScene");
    }

    public void onMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnNextLevelButtonClicked()
    {
        DOTween.KillAll();
        LevelData current = LevelRuntimePasser.Current;
        if (current == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        int lastUnlocked = PlayerPrefs.GetInt(LastUnlockedKey, 1);
        if (current.nextLevel != null)
        {
            int nextNumber = current.nextLevel.levelNumber;
            if (nextNumber > lastUnlocked)
            {
                PlayerPrefs.SetInt(LastUnlockedKey, nextNumber);
                PlayerPrefs.Save();
            }
            LevelRuntimePasser.Current = current.nextLevel;
            SceneManager.LoadScene("GameScene");
        }
    }
}
