using Verse;

namespace RobotStuff
{
    /// <summary>
    ///     Marks the ThingDef for being tweaked on initialisation.
    /// </summary>
    public class RobotEdit : DefModExtension
    {
        /// <summary>
        ///     Tweaks the corpse by removing its rotting ability.
        /// </summary>
        public bool removeCorpseRot = true;
    }
}