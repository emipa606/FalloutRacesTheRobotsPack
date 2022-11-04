using System.Reflection;
using HarmonyLib;
using Verse;

namespace FaceLaserTesting;

[StaticConstructorOnStartup]
internal class Main
{
    static Main()
    {
        var harmony = new Harmony("com.ogliss.rimworld.mod.rryatuja");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
