using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] waypoints;
    public float spawnInterval = 3f;
    public int enemiesPerWave = 5;

    [Header("Optional Settings")]
    public Transform spawnPoint; // Eğer sahnede özel bir spawn noktası kullanmak istiyorsan

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        WaypointManager wp = enemy.GetComponent<WaypointManager>();
        if (wp != null)
        {
            wp.wayPoints = waypoints;
            wp.moveSpeed = Random.Range(1.5f, 3.5f);
        }
    }
}
