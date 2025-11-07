using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TD/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Info")]
    public float damage;
    public string towerName;
    public GameObject prefab;
    public GameObject ghostPrefab;

    public GameObject nextUpgrade;
    public int upgradeCost;
    public int cost = 0;

    [Header("Placement")]
    public float yOffset = 0f; //daha sonra çakışma olursa yükselti kullanacaksa kullanılabilir
    public float footprintRadius = 0.4f;

    public float range;

    public float attackSpeed;

    public string[] upgradePopups;

    public TMP_Text textPrefab;
    public Vector3 popupOffset = new Vector3(0f, 1.0f, 0f);
    public float popupRiseDistance = 1f;
    public float popupLifeTime = 0.5f;
    public float popupInterval = 0.12f; 
}
