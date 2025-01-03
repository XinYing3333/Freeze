using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform[] spawnPoints; // 生成位置的数组
    public List<WaveItem> waves; // 存储波次的列表
    public int waveInterval; // 每波之间的时间间隔

    private int _currentWaveIndex;
    
    private void Start()
    {
        if (!GameManager.Instance.isOver)
        {
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            if (_currentWaveIndex < waves.Count)
            {
                WaveItem currentWave = waves[_currentWaveIndex];
                yield return StartCoroutine(SpawnItemWave(currentWave));

                _currentWaveIndex++;

                if (_currentWaveIndex < waves.Count)
                {
                    yield return new WaitForSeconds(waveInterval);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    IEnumerator SpawnItemWave(WaveItem wave)
    {
        AudioManager.Instance.PlayFX("WaveStart");
        foreach (WaveItemEntry entry in wave.waveEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnItem(entry.itemPrefab); 
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    void SpawnItem(GameObject itemPrefab)
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject item = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}