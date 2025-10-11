using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public LevelData level; 
    public Transform mapRoot;

    public float tileSize = 1f;
    public float decorYOffset = 0.2f; 

    [Header("Token Sets")]
    public TokenData[] terrainTokens;
    public TokenData[] decorTokens;

    Dictionary<string, TokenData> terrainMap;
    Dictionary<string, TokenData> decorMap;

    public void Build()
    {
        if (!level || !mapRoot)
        {
            Debug.LogError("Missing Data");
            return;
        }
        if (level.terrainRows == null || level.terrainRows.Length == 0)
        {
            Debug.LogError("Empty Data");
            return;
        }

        
        for (int i = mapRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(mapRoot.GetChild(i).gameObject);

        
        terrainMap = BuildMap(terrainTokens);
        decorMap   = BuildMap(decorTokens);

        int h = level.terrainRows.Length;
        int w = CountCells(level.terrainRows[0]);
        if (level.gridSize == Vector2Int.zero) level.gridSize = new Vector2Int(w, h);

        for (int y = 0; y < h; y++)
        {
            var tCells = SplitRow(level.terrainRows[y]);
            string[] dCells = null;
            if (level.decorRows != null && level.decorRows.Length > y && !string.IsNullOrWhiteSpace(level.decorRows[y]))
                dCells = SplitRow(level.decorRows[y]);

            for (int x = 0; x < w; x++)
            {
                Vector3 basePos = new Vector3(x * tileSize, 0f, (h - 1 - y) * tileSize);

                if (x < tCells.Length && terrainMap.TryGetValue(tCells[x], out var tTok) && tTok.prefab)
                {
                    var tGo = Instantiate(tTok.prefab, basePos, Quaternion.identity, mapRoot);
                    tTok.ApplyTo(tGo);
                }

                if (dCells != null && x < dCells.Length)
                {
                    string dTokStr = dCells[x];
                    if (dTokStr != "." && decorMap.TryGetValue(dTokStr, out var dTok) && dTok.prefab)
                    {
                        var dGo = Instantiate(dTok.prefab, basePos + Vector3.up * decorYOffset, Quaternion.identity, mapRoot);
                        dTok.ApplyTo(dGo); 
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
