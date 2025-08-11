using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public int startingEnemies = 5;
    public float timeBetweenWaves = 10f;
    public int waveNumber = 1;

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player;

    private bool isWaveActive = false;

    void Start()
    {
        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            int enemyCount = startingEnemies + (waveNumber * 2);
            StartCoroutine(SpawnWave(enemyCount));

            waveNumber++;
        }
    }

    IEnumerator SpawnWave(int count)
    {
        isWaveActive = true;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = GetSpawnPointAwayFromPlayer();
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            yield return new WaitForSeconds(0.5f); // Small delay between spawns
        }

        isWaveActive = false;
    }

    Transform GetSpawnPointAwayFromPlayer()
    {
        List<Transform> validSpawns = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            float distance = Vector3.Distance(player.position, point.position);
            if (distance > 15f) // Minimum safe spawn distance from player
            {
                validSpawns.Add(point);
            }
        }

        return validSpawns[Random.Range(0, validSpawns.Count)];
    }
}
