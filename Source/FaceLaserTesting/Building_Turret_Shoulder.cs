﻿using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace FaceLaserTesting
{
    public class Building_Turret_Shoulder : Building_TurretGun
    {
        public Pawn Parental;

        public bool turretIsOn;

        public CompEquippableTurret Comp
        {
            get
            {
                if (Parental != null)
                {
                    return Parental.TryGetComp<CompEquippableTurret>();
                }
                return null;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            //    Log.Message(string.Format("turret spawned"));
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            //    Log.Message(string.Format("turret despawned"));
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            //    Log.Message(string.Format("turret destroyed"));
        }
        public override void ExposeData()
        {
            base.ExposeData();
               Scribe_References.Look<Pawn>(ref Parental, "Parental", false);
            Scribe_Values.Look<bool>(ref turretIsOn, "TurretIsOn");
        }

        public override void Tick()
        {
            base.Tick();
            if (Parental == null || (Parental is Pawn pawn && (pawn.Dead || pawn.Downed)))//||this.comp==null)
            {
                Destroy();
            }
            /*
            else
            {
                if (Parental.apparel.WornApparelCount>0)
                {
                    foreach (var app in Parental.apparel.WornApparel)
                    {
                        CompEquippableTurret turretcomp = this.Parental.TryGetComp<CompEquippableTurret>();
                        if (turretcomp!=null)
                        {
                            if (this!= turretcomp.turret)
                            {
                                this.Destroy();
                            }
                        }
                    }
                }
                
            }
            */
        }
        // Token: 0x17000561 RID: 1377
        // (get) Token: 0x06002432 RID: 9266 RVA: 0x001137B0 File Offset: 0x00111BB0
        private bool CanSetForcedTarget => mannableComp != null && PlayerControlled;

        // Token: 0x17000562 RID: 1378
        // (get) Token: 0x06002433 RID: 9267 RVA: 0x001137C6 File Offset: 0x00111BC6
        private bool CanToggleHoldFire => PlayerControlled;


        // Token: 0x17000564 RID: 1380
        // (get) Token: 0x06002435 RID: 9269 RVA: 0x001137E0 File Offset: 0x00111BE0
        private bool IsMortarOrProjectileFliesOverhead => AttackVerb.ProjectileFliesOverhead();

        private bool PlayerControlled => Faction == Faction.OfPlayer;

        // Token: 0x0600244C RID: 9292 RVA: 0x00114342 File Offset: 0x00112742
        private void ResetForcedTarget()
        {
            forcedTarget = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
            if (burstCooldownTicksLeft <= 0)
            {
                TryStartShootSomething(false);
            }
        }

        // Token: 0x0600244D RID: 9293 RVA: 0x00114369 File Offset: 0x00112769
        private void ResetCurrentTarget()
        {
            currentTargetInt = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
        }

        // Token: 0x0600244E RID: 9294 RVA: 0x0011437D File Offset: 0x0011277D
        public void EditGun()
        {
            CompQuality Weapon_Quality = gun.TryGetComp<CompQuality>();
            if (Weapon_Quality != null)
            {
                Comp.parent.TryGetQuality(out QualityCategory Q);
                Weapon_Quality.SetQuality(Q, ArtGenerationContext.Outsider);
            }
            UpdateGunVerbs();
        }

        // Token: 0x0600244F RID: 9295 RVA: 0x001143A4 File Offset: 0x001127A4
        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
            for (var i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this;
                verb.castCompleteCallback = new Action(BurstComplete);
            }
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (CanSetForcedTarget)
            {
                var attack = new Command_VerbTarget
                {
                    defaultLabel = "CommandSetForceAttackTarget".Translate(),
                    defaultDesc = "CommandSetForceAttackTargetDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true),
                    verb = AttackVerb,
                    hotKey = KeyBindingDefOf.Misc4
                };
                if (Spawned && IsMortarOrProjectileFliesOverhead && Position.Roofed(Map))
                {
                    attack.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
                }
                yield return attack;
            }
            if (forcedTarget.IsValid)
            {
                var stop = new Command_Action
                {
                    defaultLabel = "CommandStopForceAttack".Translate(),
                    defaultDesc = "CommandStopForceAttackDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt", true),
                    action = delegate ()
                    {
                        ResetForcedTarget();
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    }
                };
                if (!forcedTarget.IsValid)
                {
                    stop.Disable("CommandStopAttackFailNotForceAttacking".Translate());
                }
                stop.hotKey = KeyBindingDefOf.Misc5;
                yield return stop;
            }
            /*
            if (this.CanToggleHoldFire)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "CommandHoldFire".Translate(),
                    defaultDesc = "CommandHoldFireDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire", true),
                    hotKey = KeyBindingDefOf.Misc6,
                    toggleAction = delegate ()
                    {
                        this.holdFire = !this.holdFire;
                        if (this.holdFire)
                        {
                            this.ResetForcedTarget();
                        }
                    },
                    isActive = (() => this.holdFire)
                };
            }
            */
            yield break;
        }
        public override LocalTargetInfo CurrentTarget
        {
            get
            {
                if (Parental.TargetCurrentlyAimingAt != null)
                {
                    return Parental.TargetCurrentlyAimingAt;
                }
                return base.CurrentTarget;
            }
        }

        //public override Verb AttackVerb => base.AttackVerb.EquipmentSource.TryGetQuality();
        //public override LocalTargetInfo CurrentTarget => Parental.TargetCurrentlyAimingAt != null ? Parental.TargetCurrentlyAimingAt : base.CurrentTarget;
    }
}
