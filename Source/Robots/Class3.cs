using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //Start tweaking.
            //IEnumerable<ThingDef> corpseDefs = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.defName.EndsWith("_Corpse"));

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                //If the Def got a RobotEdit do stuff, otherwise do not bother.
                RobotEdit tweaker = thingDef.GetModExtension<RobotEdit>();
                if (tweaker != null)
                {
                    ThingDef corpseDef = thingDef?.race?.corpseDef;
                    if (corpseDef != null)
                    {
                        //Removes corpse rotting.
                        if (tweaker.RobotEdit)
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
    }
}
