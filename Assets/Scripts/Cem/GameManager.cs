using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelData currentLevel;
    public int maxUnits = 5;
    private int currentUnits = 0;
    
    public int money;
    public int health;

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
        Debug.Log("Current Unit:" + currentUnits);
    }

    public void RemoveUnit()
    {
        currentUnits = Mathf.Max(currentUnits - 1, 0);
        Debug.Log("Current Unit:" + currentUnits);
    }

    #endregion


    #region Money
    public bool TrySpend(int amount)
    {
        if (money < amount) return false;

        money -= amount;
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        money += amount;
        Debug.Log("Yeni paran eklendikten sonra:" + money);
    }
    #endregion

    #region Health
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        health -= amount;
        if (health < 0) health = 0;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        health += amount;
    }
    #endregion
}
