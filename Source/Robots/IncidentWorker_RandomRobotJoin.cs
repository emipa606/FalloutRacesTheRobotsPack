using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Robots
{
    class IncidentWorker_RandomRobotJoin : IncidentWorker_WandererJoin
    {
        public static readonly List<string> allRobotPawnKindNames = new List<string>
        {
            "Robot_Assaultron_Colonist",
            "Robot_Nurse_Handy_Colonist",
            "Robot_Protectetron_Colonist",
            "Robot_ScrapBot_Colonist",
            "Robot_Securitron_Colonist",
            "Robot_Sentrybot_Colonist",
            "Robot_Mister_Gutsy_Colonist",
            "Robot_Handy_Colonist"
        };

        public override Pawn GeneratePawn()
        {
            Gender? fixedGender = null;
            if (this.def.pawnFixedGender != Gender.None)
            {
                fixedGender = new Gender?(this.def.pawnFixedGender);
            }
            var pawnKind = DefDatabase<PawnKindDef>.GetNamed(allRobotPawnKindNames.RandomElement());
            return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, false, true, 0, false, false, false, false, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null));
        }
    }
}
