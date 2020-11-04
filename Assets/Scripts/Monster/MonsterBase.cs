using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] private string monsterName;
    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private MonsterType firstType;
    [SerializeField] private MonsterType secondType;


    //base stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    [SerializeField] private List<LearnableMove> learnableMoves;

    public string MonsterName => monsterName;

    public string Description => description;

    public Sprite FrontSprite => frontSprite;

    public Sprite BackSprite => backSprite;

    public MonsterType FirstType => firstType;

    public MonsterType SecondType => secondType;

    public int MaxHp => maxHp;

    public int Attack => attack;

    public int Defense => defense;

    public int SpAttack => spAttack;

    public int SpDefense => spDefense;

    public int Speed => speed;

    public List<LearnableMove> LearnableMoves => learnableMoves;
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField]
    private MoveBase moveBase;

    [SerializeField]
    private int level;

    public MoveBase MoveBase => moveBase;

    public int Level => level;
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //These are not normal stats, they're used to boost the moveAccuracy
    Accuracy,
    Evasion
}

public enum MonsterType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}
