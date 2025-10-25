using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave 1";
    public GameObject[] enemyPrefabs;   // Bu wavede spawnlanacak düşmanlar
    public int enemyCount = 5;          // Kaç tane düşman
    public float spawnInterval = 2f;    // Kaç saniyede bir doğacak
    public float delayBeforeNextWave = 5f; // Sonraki wave'e geçmeden bekleme
}
