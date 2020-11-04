using UnityEngine;

[System.Serializable]
public class SecondaryMoveEffects : MoveEffects
{
    [SerializeField] private int chance;
    [SerializeField] private MoveTarget target;

    public int Chance => chance;
    public MoveTarget Target => target;
}
