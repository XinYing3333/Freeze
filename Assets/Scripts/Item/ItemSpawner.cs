using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10); // 生成区域的大小
    public Transform spawnAreaCenter;                      // 生成区域的中心点
    public List<WaveItem> waves;                           // 存储波次的列表
    public int waveInterval;                               // 每波之间的时间间隔

    private int _currentWaveIndex;
    private GameManager _gameManager;
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (!_gameManager.isOver)
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
        // 在范围内随机生成位置
        Vector3 randomPosition = GetRandomPositionInArea();
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameObject item = Instantiate(itemPrefab, randomPosition, randomRotation);
    }

    // 获取范围内的随机位置
    Vector3 GetRandomPositionInArea()
    {
        Vector3 halfSize = spawnAreaSize * 0.5f;
        Vector3 randomPosition = new Vector3(
            Random.Range(-halfSize.x, halfSize.x),
            Random.Range(-halfSize.y, halfSize.y),
            Random.Range(-halfSize.z, halfSize.z)
        );

        if (spawnAreaCenter != null)
        {
            randomPosition += spawnAreaCenter.position;
        }
        
        return randomPosition;
    }

    // **显示生成区域 Gizmos（新增函数）**
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(200, 0, 0, 0.3f); // 半透明绿色显示区域
        if (spawnAreaCenter != null)
        {
            Gizmos.DrawCube(spawnAreaCenter.position, spawnAreaSize);
        }
        else
        {
            Gizmos.DrawCube(transform.position, spawnAreaSize);
        }
    }
}
