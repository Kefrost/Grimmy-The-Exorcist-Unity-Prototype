using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new move")]

public class MoveBase : ScriptableObject
{
    [SerializeField] private string moveName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHits;
    [SerializeField] private int pp;
    [SerializeField] private int priority;
    [SerializeField] private MonsterType type;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveTarget target;
    [SerializeField] private MoveEffects effects;

    [SerializeField] private List<SecondaryMoveEffects> secondaryMoveEffects;

    public string MoveName => moveName;
    public string Description => description;
    public int Power => power;
    public int Accuracy => accuracy;
    public bool AlwaysHits => alwaysHits;
    public int PP => pp;
    public int Priority => priority;
    public MoveEffects Effects => effects;
    public MonsterType Type => type;
    public MoveCategory Category => category;
    public MoveTarget Target => target;

    public List<SecondaryMoveEffects> SecondaryMoveEffects => secondaryMoveEffects;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Enemy, Self
}
