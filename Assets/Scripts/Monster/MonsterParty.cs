using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters;

    public List<Monster> Monsters => monsters;

    public void InitializeMonsters()
    {
        monsters = Inventory.CurrentMonsters;

        foreach (var monster in monsters)
        {
            monster.Initialize(monster.Level);
        }
    }

    public Monster GetFirstMonster()
    {
        return monsters.Where(m => m.HP > 0).FirstOrDefault();
    }
}