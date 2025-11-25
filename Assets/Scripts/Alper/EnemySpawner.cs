using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelData levelData;

    [Header("Enemy Token Data")]
    public List<TokenData> tokenDataList = new List<TokenData>();

    private Dictionary<string, TokenData> tokenDictionary;
    private Transform[] waypoints;
    public Button spawnButton; 
    private bool isWaveActive = false; 
    private int currentWaveIndex = 0; 

    void Awake()
    {
        levelData = LevelRuntimePasser.Current;
        tokenDictionary = new Dictionary<string, TokenData>();
        foreach (var tokenData in tokenDataList)
        {
            if (!tokenDictionary.ContainsKey(tokenData.token))
                tokenDictionary.Add(tokenData.token, tokenData);
        }
    }

    void Start()
    {
        if (PathProvider.Instance != null)
            waypoints = PathProvider.Instance.GetWaypoints();

        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(OnSpawnButtonClicked);
            spawnButton.interactable = true; 
        }
    }

    public void OnSpawnButtonClicked()
    {
        if (!isWaveActive && currentWaveIndex < levelData.waves.Count)
        {
            GameManager.Instance?.SetWaveActive(true);
            StartCoroutine(SpawnWave(levelData.waves[currentWaveIndex])); 
            isWaveActive = true;
            spawnButton.interactable = false; 
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
        yield return new WaitUntil(CheckWaveCompletion);
        currentWaveIndex++; 
        if (currentWaveIndex >= levelData.waves.Count)
            {
                if(GameManager.Instance?.getHealth() > 0)
            {
                GameManager.Instance?.onWin();
            }

            }
    }
    public void SpawnEnemyByToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;
        if (!tokenDictionary.ContainsKey(token))
        {
            return;
        }
        TokenData tokenData = tokenDictionary[token];
        GameObject prefab = tokenData.prefab;
       Vector3 spawnPosition;
        if (levelData != null && levelData.waypointPositions != null && levelData.waypointPositions.Count > 0)
        {
            spawnPosition = levelData.waypointPositions[0];
        }
        else
        {
            spawnPosition = transform.position;
        }
        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        tokenData.ApplyTo(enemy);

        WaypointManager wp = enemy.GetComponent<WaypointManager>();
        if (wp != null && PathProvider.Instance != null)
        {
            wp.wayPoints = new List<Transform>(PathProvider.Instance.GetWaypoints());
        }
    }

    private bool CheckWaveCompletion()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            isWaveActive = false;
            spawnButton.interactable = true;
            GameManager.Instance?.SetWaveActive(false);
            return true;
        }
        return false;
    }
}
