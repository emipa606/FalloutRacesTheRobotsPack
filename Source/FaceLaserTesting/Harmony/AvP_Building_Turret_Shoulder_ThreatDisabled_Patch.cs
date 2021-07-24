using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace FaceLaserTesting
{
    [HarmonyPatch(typeof(Building_Turret), "ThreatDisabled")]
    public static class AvP_Building_Turret_Shoulder_ThreatDisabled_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreShoulderTurret(Building_Turret_Shoulder __instance, ref bool __result,
            IAttackTargetSearcher disabledFor)
        {
            var unused = Find.Selector.SelectedObjects.Contains(__instance);
            var shouldturret = false;
            if (__instance != null)
            {
                if (__instance.GetType() == typeof(Building_Turret_Shoulder))
                {
                    shouldturret = true;
                }
            }

            __result = __result || shouldturret;
        }
    }
}