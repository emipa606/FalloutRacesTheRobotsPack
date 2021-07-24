using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FaceLaserTesting
{
    [HarmonyPatch(typeof(Apparel), "GetWornGizmos")]
    public static class Ogliss_RimWorld_Apparel_GetWornGizmos
    {
        [HarmonyPostfix]
        public static void ApparelGizmosFromComps(Apparel __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance == null)
            {
                //    Log.Warning("ApparelGizmosFromComps cannot access Apparel.");
                return;
            }

            if (__result == null)
            {
                //    Log.Warning("ApparelGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            var l = new List<Gizmo>(__result);
            foreach (var comp in __instance.GetComps<CompWearable>())
            {
                foreach (var gizmo in comp.CompGetGizmosWorn())
                {
                    l.Add(gizmo);
                }
            }

            __result = l;
        }
    }
}