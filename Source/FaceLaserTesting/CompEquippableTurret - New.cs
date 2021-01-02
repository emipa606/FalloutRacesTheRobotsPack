﻿using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FaceLaserTesting
{
    public class CompProperties_EquippableTurret : CompProperties
    {
        public CompProperties_EquippableTurret()
        {
            compClass = typeof(CompEquippableTurret);
        }
        public bool OnByDefault = false;
        public bool DisableInMelee = true;
        public ThingDef TurretDef = null;
    }

    // Token: 0x02000E58 RID: 3672
    public class CompEquippableTurret : CompWearable
    {
        public CompProperties_EquippableTurret Props => (CompProperties_EquippableTurret)props;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
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

        public bool Toggled = true;
        public bool DisableInMelee = true;

        public bool OnDefault => Props.OnByDefault;
        public bool MeleeDisable => Props.DisableInMelee;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Thing>(ref turret, "TurretThing", false);
            Scribe_Deep.Look<Building_Turret_Shoulder>(ref turret_Shoulder, "TurretBuilding", false);
            Scribe_Values.Look(ref Toggled, "ToggledMode", OnDefault, true);
            Scribe_Values.Look(ref DisableInMelee, "DisableInMelee", MeleeDisable, true);
            //    Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn", IsTurnedOn);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (IsWorn && GetWearer.Map != null)
            {
                
                if (TurretIsOn || Find.TickManager.TicksGame >= nextUpdateTick)
                {
                    nextUpdateTick = Find.TickManager.TicksGame + 60;
                    RefreshTurretState();
                }
            }
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            parent.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
        public void RefreshTurretState()
        {
            if (ComputeTurretState())
            {
                SwitchOnTurret();
                return;
            }
            SwitchOffTurret();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
        public bool ComputeTurretState()
        {
            return GetWearer != null && !GetWearer.Dead && !GetWearer.Downed && GetWearer.Awake() && TurretIsOn;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void SwitchOnTurret()
        {
            var intVec = GetWearer.DrawPos.ToIntVec3();
            if (!turret.DestroyedOrNull() && intVec != turret.Position)
            {
                MoveTurret(intVec);
            }
            if ((turret.DestroyedOrNull() || !turret.Spawned) && intVec.GetFirstThing(GetWearer.Map, Props.TurretDef) == null)
            {
                turret = GenSpawn.Spawn(Props.TurretDef, intVec, GetWearer.Map, WipeMode.Vanish);
                turret.SetFactionDirect(GetWearer.Faction);
                ((Building_Turret_Shoulder)turret).Parental = GetWearer;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void SwitchOffTurret()
        {
            if (!turret.DestroyedOrNull() && turret.Spawned)
            {
                turret.DeSpawn();
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void MoveTurret(IntVec3 intVec)
        {
            if (!turret.DestroyedOrNull())
            {
                turret.Position = intVec;
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            var flag = Find.Selector.SelectedObjects.Contains(GetWearer);
            if (flag && GetWearer.IsColonist)
            {
                Texture2D CommandTex;
                var num = 700000101;
                if (Toggled)
                {
                    CommandTex = ContentFinder<Texture2D>.Get("GUI/Commands/CommandButton_On", true);
                }
                else
                {
                    CommandTex = ContentFinder<Texture2D>.Get("GUI/Commands/CommandButton_Off", true);
                }
                yield return new Command_Toggle
                {

                    icon = CommandTex,
                    defaultLabel = Toggled ? "Turret: on." : "Turret: off.",
                    defaultDesc = "Switch mode.",
                    isActive = () => Toggled,
                    toggleAction = delegate ()
                    {
                        Toggled = !Toggled;
                        SwitchTurretMode();
                    },
                    activateSound = SoundDef.Named("Click"),
                    groupKey = num + 1,
                    /*
                    disabled = GetWearer.stances.curStance.StanceBusy,
                    disabledReason = "Busy"
                    */
                };
            }
            yield break;
        }


        // Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
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

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 60;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;

        // Token: 0x04000003 RID: 3 Building_Turret_Shoulder
        public Thing turret;
        public Building_Turret_Shoulder turret_Shoulder;

        // Token: 0x04000004 RID: 4

    }
}
