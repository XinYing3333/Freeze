using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform[] spawnPoints; // 敌人生成位置的数组
    public List<Wave> waves; // 存储波次的列表
    public int waveInterval; // 每波之间的时间间隔

    public Transform target;
    
    private int _currentWaveIndex = 0;
    
    public Text countdownText; 
    

    private void Start()
    {
        countdownText.text = "";
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
                Wave currentWave = waves[_currentWaveIndex];
                yield return StartCoroutine(SpawnEnemyWave(currentWave));
                
                _currentWaveIndex++;
                
                if (_currentWaveIndex < waves.Count)
                {
                    // 开始倒计时
                    yield return StartCoroutine(StartCountdown());
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
        AudioManager.Instance.PlayFX("WaveStart");
        foreach (WaveEntry entry in wave.waveEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyPrefab);
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    private IEnumerator StartCountdown()
    {
        int currentTime = waveInterval;
        
        while (currentTime > 0)
        {
            countdownText.text = "Wave " + (_currentWaveIndex + 1).ToString("") + " arrive in:  " + currentTime.ToString(""); // Format the time to one decimal place

            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        countdownText.text = "";
    }
}
