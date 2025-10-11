using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TD/Level Data")]
public class LevelData : ScriptableObject
{


    public string levelName;
    public Vector2Int gridSize; 

    [Header("Terrain Layer")]
    public string[] terrainRows;

    [Header("Decor Layer")]
    
    public string[] decorRows;
}
