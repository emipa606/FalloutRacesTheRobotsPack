using System;
using System.Collections.Generic;
using System.Linq;
using AchievementsExpanded;
using RimWorld;
using Verse;

namespace Robots
{
    public class RobotCurrentTracker : TrackerBase
    {
        public static readonly List<string> allRobotDefNames = new List<string>
        {
            "Assaultron",
            "MrHandy",
            "MisterGutsy",
            "NurseHandy",
            "Protectetron",
            "ScrapBot",
            "Securitron",
            "Sentrybot"
        };

        public int count = 1;

        [Unsaved] protected int triggeredCount; //Only for display

        public RobotCurrentTracker()
        {
        }

        public RobotCurrentTracker(RobotCurrentTracker reference) : base(reference)
        {
            count = reference.count;
        }

        public override string Key => "RobotCurrentTracker";

        public override Func<bool> AttachToLongTick => Trigger;

        protected override string[] DebugText => new[] {$"Count: {count}"};

        public override (float percent, string text) PercentComplete => count > 1
            ? ((float) triggeredCount / count, $"{triggeredCount} / {count}")
            : base.PercentComplete;


        public override bool UnlockOnStartup => Trigger();


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref count, "count", 1);
        }

        public override bool Trigger()
        {
            base.Trigger();
            var factionPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction;
            if (factionPawns is null)
            {
                return false;
            }

            var robots = from robot in factionPawns where allRobotDefNames.Contains(robot.def.defName) select robot;
            triggeredCount = robots.Count();

            return triggeredCount >= count;
        }
    }
}