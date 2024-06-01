using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform[] spawnPoints; // 敌人生成位置的数组
    public List<Wave> waves; // 存储波次的列表
    public float waveInterval = 10f; // 每波之间的时间间隔

    public Transform target;
    
    private int _currentWaveIndex = 0;
    private bool spawningWave = false;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            if (_currentWaveIndex < waves.Count)
            {
                Wave currentWave = waves[_currentWaveIndex];
                yield return StartCoroutine(SpawnEnemyWave(currentWave));
                _currentWaveIndex++;
                if (_currentWaveIndex < waves.Count)
                {
                    yield return new WaitForSeconds(waveInterval); // 在波次生成完成後等待
                }
            }
            else
            {
                Debug.Log("All waves completed.");
                yield break;
            }
        }
    }

    IEnumerator SpawnEnemyWave(Wave wave)
    {
        spawningWave = true;

        foreach (WaveEntry entry in wave.waveEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyPrefab);
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        spawningWave = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemyCtrl enemyCtrl = enemy.GetComponent<EnemyCtrl>();
        if (enemyCtrl != null)
        {
            enemyCtrl.SetTarget(target); // 设置敌人的目标
        }
    }
}