using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    [Header("Basic")]
    public string waveName;
    public float spawnInterval = 0.5f;
    public List<string> enemyTokens = new List<string>();
}