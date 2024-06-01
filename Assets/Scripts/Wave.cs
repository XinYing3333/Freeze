using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public List<WaveEntry> waveEntries;
    public float spawnInterval = 5f;
}

[System.Serializable]
public class WaveEntry
{
    public GameObject enemyPrefab;
    public int count;
}