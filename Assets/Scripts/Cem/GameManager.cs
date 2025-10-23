using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
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
        if(currentUnitText != null)
        {
            defaultUnitColor = currentUnitText.color;
        }
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
        if (health < 0) health = 0;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = health.ToString();
    }
    #endregion
}
