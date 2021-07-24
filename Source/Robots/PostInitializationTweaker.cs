using RimWorld;
using Verse;

namespace RobotStuff
{
    /// <summary>
    ///     Tweaks ThingDefs after the game has been made.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class PostInitializationTweaker
    {
        static PostInitializationTweaker()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                //If the Def has a RobotEdit do blank, otherwise does nothing.
                var tweaker = thingDef.GetModExtension<RobotEdit>();
                if (tweaker == null)
                {
                    continue;
                }

                var corpseDef = thingDef.race?.corpseDef;
                if (corpseDef == null)
                {
                    continue;
                }

                //Removes the corpse rotting.
                if (!tweaker.removeCorpseRot)
                {
                    continue;
                }

                corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
            }
        }
    }
}