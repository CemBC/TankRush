using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TD/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelHealth;
    public int levelStartupMoney;
    public int maxUnits;
    public string levelName;
    public int levelNumber;
    public Vector2Int gridSize;

    [Header("Terrain Layer")]
    public string[] terrainRows;

    [Header("Decor Layer")]
    public string[] decorRows;

    public List<Vector3> waypointPositions = new List<Vector3>();
    public List<WaveData> waves = new List<WaveData>();

    public List<bool> unitAvailability = new List<bool>();

    public LevelData nextLevel;
}
