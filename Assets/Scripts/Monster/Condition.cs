using System;
public class Condition
{
    public ConditionId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Message { get; set; }

    public Action<Monster> OnStart { get; set; }
    public Func<Monster, bool> OnBeforeMove { get; set; }
    public Action<Monster> OnAfterTurn { get; set; }
}
