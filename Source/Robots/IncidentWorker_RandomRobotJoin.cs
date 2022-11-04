using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Robots;

internal class IncidentWorker_RandomRobotJoin : IncidentWorker_WandererJoin
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
        if (def.pawnFixedGender != Gender.None)
        {
            fixedGender = def.pawnFixedGender;
        }

        var pawnKind = DefDatabase<PawnKindDef>.GetNamed(allRobotPawnKindNames.RandomElement());
        return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, Faction.OfPlayer,
            PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, 0, false, false, allowFood: false,
            allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false,
            forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f,
            biocodeApparelChance: 0, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1f,
            validatorPreGear: null, validatorPostGear: null, forcedTraits: null, prohibitedTraits: null,
            minChanceToRedressWorldPawn: null, fixedBiologicalAge: null, fixedChronologicalAge: null,
            fixedGender: fixedGender));
    }
}