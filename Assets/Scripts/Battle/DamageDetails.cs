public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float Effectiveness { get; set; }

    public DamageDetails(float critical, float eff)
    {
        Critical = critical;
        Effectiveness = eff;
        Fainted = false;
    }
}
