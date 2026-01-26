using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave System/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Configuration")]
    [SerializeField] private List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
    
    public List<EnemySpawnInfo> Enemies => enemies;
    
    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var enemyInfo in enemies)
        {
            total += enemyInfo.count;
        }
        return total;
    }
}
