using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] private int level;

    [SerializeField] private MonsterBase mBase;

    public bool HpChanged { get; set; }
    public int Level { get { return level; } set { level = value; } }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public int HP { get; set; }
    public Condition VolatileStatus { get; private set; }
    public Condition Status { get; private set; }
    public MonsterBase MBase => mBase;
    public Move CurrentMove { get; set; }

    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Queue<string> StatusChanges { get; private set; }

    public event Action OnStatusChanged;

    public int Attack => GetStat(Stat.Attack);
    public int Defense => GetStat(Stat.Defense);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefense => GetStat(Stat.SpDefense);
    public int Speed => GetStat(Stat.Speed);
    public int MaxHp { get; private set; }

    public void Initialize(int lvl)
    {
        level = lvl;

        StatusChanges = new Queue<string>();

        Moves = new List<Move>();

        var learnableMovesSorted = mBase.LearnableMoves.OrderByDescending(m => m.Level).ToList();

        foreach (var move in learnableMovesSorted)
        {
            if (level >= move.Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }

            if (Moves.Count >= 4)
            {
                break;
            }
        }

        CalculateStats();
        HP = MaxHp;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public void LevelUp()
    {
        Level += 5;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
    }

    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt((MBase.Attack * Level) / 100f) + 5 },
            { Stat.Defense, Mathf.FloorToInt((MBase.Defense * Level) / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt((MBase.SpAttack * Level) / 100f) + 5 },
            { Stat.SpDefense, Mathf.FloorToInt((MBase.SpDefense * Level) / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt((MBase.Speed * Level) / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt((MBase.MaxHp * Level) / 100f) + 10 + Level;
    }

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        //Apply a stat boost
        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{MBase.MonsterName}'s {stat} rose!");
            }
            else if (boost < 0)
            {
                StatusChanges.Enqueue($"{MBase.MonsterName}'s {stat} fell!");
            }
        }
    }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        int attack = move.Base.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        int defense = move.Base.Category == MoveCategory.Special ? SpDefense : Defense;

        float critical = 1f;

        if (UnityEngine.Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        float effectiveness = TypeChart.GetEffectiveness(move.Base.Type, this.MBase.FirstType) * TypeChart.GetEffectiveness(move.Base.Type, this.MBase.SecondType);

        DamageDetails damageDetails = new DamageDetails(critical, effectiveness);

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * effectiveness * critical;

        float a = (2 * attacker.Level + 10) / 250f;

        float d = a * move.Base.Power * ((float)attack / defense) + 2;

        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHp(damage);
        
        return damageDetails;
    }

    public bool OnBeforeTurn()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void SetStatus(ConditionId conditionId)
    {
        if (Status != null)
        {
            return;
        }
        Status = ConditionsDb.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{mBase.MonsterName} {Status.Message}");
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionId conditionId)
    {
        if (Status != null)
        {
            return;
        }
        VolatileStatus = ConditionsDb.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{mBase.MonsterName} {VolatileStatus.Message}");
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        List<Move> movesWitchPp = Moves.Where(m => m.PP > 0).ToList();

        int index = UnityEngine.Random.Range(0, movesWitchPp.Count);
        return movesWitchPp[index];
    }

    public void UpdateHp(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void Heal(int heal)
    {
        int healAmount = HP + heal;
        if (healAmount >= this.MaxHp)
        {
            healAmount = 0;
        }

        HP = Mathf.Clamp(HP + healAmount, 0, MaxHp);
        HpChanged = true;
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}
