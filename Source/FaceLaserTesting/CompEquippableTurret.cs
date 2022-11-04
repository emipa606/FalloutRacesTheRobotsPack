using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FaceLaserTesting;

public class CompEquippableTurret : CompWearable
{
    public const int updatePeriodInTicks = 60;
    public bool DisableInMelee = true;

    public int nextUpdateTick;

    public bool Toggled = true;

    public Thing turret;
    public Building_Turret_Shoulder turret_Shoulder;
    public CompProperties_EquippableTurret Props => (CompProperties_EquippableTurret)props;

    // Determine who is wearing this ThingComp. Returns a Pawn or null.
    protected virtual Pawn GetWearer
    {
        get
        {
            switch (ParentHolder)
            {
                case Pawn_ApparelTracker:
                    return (Pawn)ParentHolder.ParentHolder;
                case Pawn_EquipmentTracker:
                    return (Pawn)ParentHolder.ParentHolder;
                default:
                    return null;
            }
        }
    }

    // Determine if this ThingComp is being worn presently. Returns True/False
    protected virtual bool IsWorn => GetWearer != null;

    public bool TurretIsOn
    {
        get
        {
            if (!IsWorn)
            {
                return false;
            }

            if (GetWearer.Dead || GetWearer.Downed || GetWearer.IsPrisoner)
            {
                return false;
            }

            if (GetWearer.mindState.MeleeThreatStillThreat && DisableInMelee)
            {
                return false;
            }

            return GetWearer.Awake() && Toggled;
        }
    }

    public bool OnDefault => Props.OnByDefault;
    public bool MeleeDisable => Props.DisableInMelee;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_References.Look(ref turret, "TurretThing");
        Scribe_Deep.Look(ref turret_Shoulder, "TurretBuilding", false);
        Scribe_Values.Look(ref Toggled, "ToggledMode", OnDefault, true);
        Scribe_Values.Look(ref DisableInMelee, "DisableInMelee", MeleeDisable, true);
    }

    public override void CompTick()
    {
        base.CompTick();
        if (!IsWorn || GetWearer.Map == null)
        {
            return;
        }

        if (!TurretIsOn && Find.TickManager.TicksGame < nextUpdateTick)
        {
            return;
        }

        nextUpdateTick = Find.TickManager.TicksGame + 60;
        RefreshTurretState();
    }

    public void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        parent.SpawnSetup(map, respawningAfterLoad);
        if (!respawningAfterLoad)
        {
            nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
        }
    }

    public void RefreshTurretState()
    {
        if (ComputeTurretState())
        {
            SwitchOnTurret();
            return;
        }

        SwitchOffTurret();
    }

    public bool ComputeTurretState()
    {
        return GetWearer is { Dead: false, Downed: false } && GetWearer.Awake() && TurretIsOn;
    }

    public void SwitchOnTurret()
    {
        var intVec = GetWearer.DrawPos.ToIntVec3();
        if (!turret.DestroyedOrNull() && intVec != turret.Position)
        {
            MoveTurret(intVec);
        }

        if (!turret.DestroyedOrNull() && turret.Spawned ||
            intVec.GetFirstThing(GetWearer.Map, Props.TurretDef) != null)
        {
            return;
        }

        turret = GenSpawn.Spawn(Props.TurretDef, intVec, GetWearer.Map);
        turret.SetFactionDirect(GetWearer.Faction);
        ((Building_Turret_Shoulder)turret).Parental = GetWearer;
    }

    public void SwitchOffTurret()
    {
        if (!turret.DestroyedOrNull() && turret.Spawned)
        {
            turret.DeSpawn();
        }
    }

    public void MoveTurret(IntVec3 intVec)
    {
        if (!turret.DestroyedOrNull())
        {
            turret.Position = intVec;
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosWorn()
    {
        var unused = IsWorn ? GetWearer : parent;
        if (!Find.Selector.SelectedObjects.Contains(GetWearer) || !GetWearer.IsColonist)
        {
            yield break;
        }

        var num = 700000101;
        var CommandTex =
            ContentFinder<Texture2D>.Get(Toggled
                ? "GUI/Commands/CommandButton_On"
                : "GUI/Commands/CommandButton_Off");

        yield return new Command_Toggle
        {
            icon = CommandTex,
            defaultLabel = Toggled ? "Turret: on." : "Turret: off.",
            defaultDesc = "Switch mode.",
            isActive = () => Toggled,
            toggleAction = delegate
            {
                Toggled = !Toggled;
                SwitchTurretMode();
            },
            activateSound = SoundDef.Named("Click"),
            groupKey = num + 1
        };
    }


    public void SwitchTurretMode()
    {
        switch (TurretIsOn)
        {
            case true:
                SwitchOffTurret();
                break;
            case false:
                SwitchOnTurret();
                break;
        }

        RefreshTurretState();
    }
}