using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public LevelData level; 
    public Transform mapRoot;

    public float tileSize = 1f;
    public float decorYOffset = 0.2f; 

    [Header("Tokens")]
    public TokenData[] terrainTokens;
    public TokenData[] decorTokens;

    Dictionary<string, TokenData> terrainMap;
    Dictionary<string, TokenData> decorMap;

    public void Build()
    {
        if (!level || !mapRoot){
            Debug.LogError("Missing Data");
            return;
        }
        if (level.terrainRows == null || level.terrainRows.Length == 0){
            Debug.LogError("Empty Data");
            return;
        }

        for (int i = mapRoot.childCount - 1; i >= 0; i--){
            DestroyImmediate(mapRoot.GetChild(i).gameObject);
        }
        
        terrainMap = BuildMap(terrainTokens);
        decorMap   = BuildMap(decorTokens);

        int height = level.terrainRows.Length;
        int width = CountCells(level.terrainRows[0]);
        if (level.gridSize == Vector2Int.zero)
        {
            level.gridSize = new Vector2Int(width, height);
        }
        
        for (int y = 0; y < height; y++)
        {
            var terrainCells = SplitRow(level.terrainRows[y]);
            string[] decorCells = null;
            if (level.decorRows != null &&
            level.decorRows.Length > y && !string.IsNullOrWhiteSpace(level.decorRows[y]))
            {
                decorCells = SplitRow(level.decorRows[y]);
            }
            for (int x = 0; x < width; x++)
            {
                Vector3 basePosition = new Vector3(x * tileSize, 0f, (height - 1 - y) * tileSize);

                if (x < terrainCells.Length && terrainMap.TryGetValue(terrainCells[x], out var terrainToken) && terrainToken.prefab)
                {
                    var terrainGameObject = Instantiate(terrainToken.prefab, basePosition, Quaternion.identity, mapRoot);
                    terrainToken.ApplyTo(terrainGameObject);
                }

                if (decorCells != null && x < decorCells.Length)
                {
                    string decorTokenString = decorCells[x];
                    if (decorTokenString != "." && decorMap.TryGetValue(decorTokenString, out var decorToken) && decorToken.prefab)
                    {
                        var decorGameObject = Instantiate(decorToken.prefab, basePosition + Vector3.up * decorYOffset, Quaternion.identity, mapRoot);
                        decorToken.ApplyTo(decorGameObject); 
                    }
                }
            }
        }
    }

    public Dictionary<string, TokenData> BuildMap(TokenData[] list)
    {
        var dict = new Dictionary<string, TokenData>();
        if (list == null) return dict;
        foreach (var t in list)
        {
            if (t != null && !string.IsNullOrEmpty(t.token))
                dict[t.token] = t;
        }
        return dict;
    }

    public string[] SplitRow(string row) =>
        row.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

    public int CountCells(string row) => SplitRow(row).Length;

    public void Start()
    {
        Build();
    }
}
