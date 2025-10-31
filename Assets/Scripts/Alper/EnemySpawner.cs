using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelData levelData;
    public Transform spawnPoint; // Haritanın başı

    private Transform[] waypoints;

    void Start()
    {
        if (PathProvider.Instance != null)
            waypoints = PathProvider.Instance.GetWaypoints();

        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        for (int i = 0; i < levelData.waves.Count; i++)
        {
            WaveData wave = levelData.waves[i];
            Debug.Log($"🌊 Başladı: {wave.waveName}");

            yield return StartCoroutine(SpawnWave(wave));

            Debug.Log($"✅ {wave.waveName} tamamlandı.");
            yield return new WaitForSeconds(wave.delayBeforeNextWave);
        }

        Debug.Log("🎉 Tüm wave'ler tamamlandı!");
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnEnemy(WaveData wave)
{
    if (wave.enemyPrefabs.Length == 0) return;

    GameObject prefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];
    Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;

    GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

    // 👇 Ekledik — her düşmanı doğar doğmaz resetliyor
    enemy.transform.localScale = Vector3.one;
    enemy.transform.rotation = Quaternion.identity;

    WaypointManager wp = enemy.GetComponent<WaypointManager>();
    if (wp != null && PathProvider.Instance != null)
    {
        wp.wayPoints = new System.Collections.Generic.List<Transform>(PathProvider.Instance.GetWaypoints());
        wp.moveSpeed = Random.Range(1.5f, 3f);
    }
}
}
