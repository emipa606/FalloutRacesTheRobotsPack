using RimWorld;
using Verse;

namespace RobotStuff
{
    /// <summary>
    /// Tweaks ThingDefs after the game has been made.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class PostInitializationTweaker
    {
        static PostInitializationTweaker()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                //If the Def has a RobotEdit do blank, otherwise does nothing.
                RobotEdit tweaker = thingDef.GetModExtension<RobotEdit>();
                if (tweaker != null)
                {
                    ThingDef corpseDef = thingDef?.race?.corpseDef;
                    if (corpseDef != null)
                    {
                        //Removes the corpse rotting.
                        if (tweaker.removeCorpseRot)
                        {
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
                        }

                            }
                        }
                    }
                }
            }
        }