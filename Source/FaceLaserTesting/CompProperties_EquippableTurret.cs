using Verse;

namespace FaceLaserTesting;

public class CompProperties_EquippableTurret : CompProperties
{
    public bool DisableInMelee = true;
    public bool OnByDefault = false;
    public ThingDef TurretDef = null;

    public CompProperties_EquippableTurret()
    {
        compClass = typeof(CompEquippableTurret);
    }
}