using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelData levelData;
    public Transform spawnPoint;

    [Header("Enemy Token Data")]
    public List<TokenData> tokenDataList = new List<TokenData>();

    private Dictionary<string, TokenData> tokenDict;
    private Transform[] waypoints;

    void Awake()
    {
        tokenDict = new Dictionary<string, TokenData>();
        foreach (var tokenData in tokenDataList)
        {
            if (!tokenDict.ContainsKey(tokenData.token))
                tokenDict.Add(tokenData.token, tokenData);
        }
    }

    void Start()
    {
        if (PathProvider.Instance != null)
            waypoints = PathProvider.Instance.GetWaypoints();

        StartCoroutine(SpawnAllWaves());
    }

    public IEnumerator SpawnAllWaves()
    {
        for (int i = 0; i < levelData.waves.Count; i++)
        {
            WaveData wave = levelData.waves[i];
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(wave.delayBeforeNextWave);
        }
    }

    public IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.enemyTokens.Count; i++)
        {
            string token = wave.enemyTokens[i];
            SpawnEnemyByToken(token);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    public void SpawnEnemyByToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;
        if (!tokenDict.ContainsKey(token))
        {
            Debug.LogWarning($"SpawnEnemyByToken: Bu token için prefab bulunamadı: {token}");
            return;
        }
        TokenData tokenData = tokenDict[token];
        GameObject prefab = tokenData.prefab;
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        tokenData.ApplyTo(enemy);
        WaypointManager wp = enemy.GetComponent<WaypointManager>();
        if (wp != null && PathProvider.Instance != null)
        {
            wp.wayPoints = new List<Transform>(PathProvider.Instance.GetWaypoints());
        }
    }
}
