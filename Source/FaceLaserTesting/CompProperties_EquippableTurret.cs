using Verse;

namespace FaceLaserTesting
{
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

    // Token: 0x02000E58 RID: 3672
}