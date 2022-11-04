using Verse;

namespace RobotRamRod;

public class RobotCompProperties_Hatcher : CompProperties
{
    public float hatcherDaystoHatch = 1f;

    public PawnKindDef hatcherPawn;

    public RobotCompProperties_Hatcher()
    {
        compClass = typeof(RobotCompHatcher);
    }
}