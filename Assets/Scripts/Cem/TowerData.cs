using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TD/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Info")]
    public string towerName;
    public GameObject prefab;
    public GameObject ghostPrefab;  
    public int cost = 0;

    [Header("Placement")]
    public float yOffset = 0f; //daha sonra çakışma olursa yükselti kullanacaksa kullanılabilir
    public float footprintRadius = 0.4f;

    public float range;

    public float attackSpeed;
}
