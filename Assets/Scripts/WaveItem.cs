using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveItem
{
    public List<WaveItemEntry> waveEntries;
    public float spawnInterval = 5f;
}

[System.Serializable]
public class WaveItemEntry
{
    public GameObject itemPrefab;
    public int count;
}