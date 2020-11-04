using System.Collections.Generic;

public class ConditionsDb
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            ConditionId conditionId = kvp.Key;
            Condition condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionId, Condition> Conditions { get; private set; } = new Dictionary<ConditionId, Condition>()
    {
        {ConditionId.Psn,
            new Condition()
            {
                Name = "Poison",
                Message = "has been poisoned somehow...",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHp(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} hurt itself due to poison");
                }
            }
        },
        {ConditionId.Brn,
            new Condition()
            {
                Name = "Burn",
                Message = "has been burned somehow...",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHp(monster.MaxHp / 16);
                    monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} hurt itself due to burn");
                }
            }
        },
        {ConditionId.Par,
            new Condition()
            {
                Name = "Paralyzed",
                Message = "has been paralyzed",
                OnBeforeMove = (Monster monster) =>
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName}'s paralyzed and can't move");
                        return false;
	                }

                    return true;
                }
            }
        },
        {ConditionId.Frz,
            new Condition()
            {
                Name = "Freeze",
                Message = "has been frozen somehow...",
                OnBeforeMove = (Monster monster) =>
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName}'s not frozen anymore");
                        return true;
                    }

                    return false;
                }
            }
        },
        {ConditionId.Slp,
            new Condition()
            {
                Name = "Sleep",
                Message = "has fallen asleep",
                OnStart = (Monster monster) =>
                {
                    monster.StatusTime = UnityEngine.Random.Range(1, 4);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.StatusTime <= 0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} woke up!");
                        return true;
                    }

                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} is sleeping");
                    return false;
                }
            }
        },

        // Volatile status conditions
        {ConditionId.Confusion,
            new Condition()
            {
                Name = "Confusion",
                Message = "has been confused",
                OnStart = (Monster monster) =>
                {
                    //Confused for 1 - 4 turns
                    monster.VolatileStatusTime = UnityEngine.Random.Range(1, 4);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} kicked out of confusion!");
                        return true;
                    }

                    monster.VolatileStatusTime--;

                    //50% chance to do a move
                    if (UnityEngine.Random.Range(1, 3) == 1)
                    {
                        return true;
	                }

                    //Hurt by confusion
                    monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} is confused");
                    monster.UpdateHp(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue($"{monster.MBase.MonsterName} hurt itself due to confusion");
                    return false;
                }
            }
        }
    };
}

public enum ConditionId
{
    None, Psn, Brn, Slp, Par, Frz, Confusion
}
