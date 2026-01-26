using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Wave System Config", menuName = "Wave System/Wave System Config")]
public class WaveSystemConfig : ScriptableObject
{
    [Header("Wave Configuration")]
    [SerializeField] private List<WaveData> waves = new List<WaveData>();
    
    [Header("Spawn Settings")]
    [SerializeField] private int initialSpawnCount = 4;
    [SerializeField] private int continuousSpawnCount = 2;
    [SerializeField] private float spawnInterval = 6f;
    
    public List<WaveData> Waves => waves;
    public int InitialSpawnCount => initialSpawnCount;
    public int ContinuousSpawnCount => continuousSpawnCount;
    public float SpawnInterval => spawnInterval;
}
