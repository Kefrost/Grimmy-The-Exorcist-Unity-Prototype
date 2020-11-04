public class Move
{
    public int PP { get; set; }
    public MoveBase Base { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;

        PP = mBase.PP;
    }
}
