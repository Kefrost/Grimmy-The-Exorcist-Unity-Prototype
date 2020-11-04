using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private ConditionId status;
    [SerializeField] private ConditionId volatileStatus;
    [SerializeField] private List<StatBoost> boosts;

    public ConditionId Status => status;
    public ConditionId VolatileStatus => volatileStatus;
    public List<StatBoost> Boosts => boosts;
}
