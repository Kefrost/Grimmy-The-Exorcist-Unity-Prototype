using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private List<MonsterPrefab> monsters;

    public MonsterPrefab GetRandomMonster()
    {
        MonsterPrefab monster = monsters[Random.Range(0, monsters.Count)];
        int level = Random.Range(Player.CurrentStage * 10, Player.CurrentStage * 20 + 1);
        
        monster.Monster.Initialize(level);
        return monster;
    }
}
