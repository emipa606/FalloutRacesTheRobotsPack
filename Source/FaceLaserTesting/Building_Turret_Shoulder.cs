using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace FaceLaserTesting;

public class Building_Turret_Shoulder : Building_TurretGun
{
    public Pawn Parental;

    public bool turretIsOn;

    public CompEquippableTurret Comp => Parental?.TryGetComp<CompEquippableTurret>();

    private bool CanSetForcedTarget => mannableComp != null && PlayerControlled;

    private bool CanToggleHoldFire => PlayerControlled;


    private bool IsMortarOrProjectileFliesOverhead => AttackVerb.ProjectileFliesOverhead();

    private bool PlayerControlled => Faction == Faction.OfPlayer;

    public override LocalTargetInfo CurrentTarget => Parental.TargetCurrentlyAimingAt != null
        ? Parental.TargetCurrentlyAimingAt
        : base.CurrentTarget;

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        base.Destroy(mode);
        //    Log.Message(string.Format("turret destroyed"));
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref Parental, "Parental");
        Scribe_Values.Look(ref turretIsOn, "TurretIsOn");
    }

    public override void Tick()
    {
        base.Tick();
        if (Parental == null || Parental is { } pawn && (pawn.Dead || pawn.Downed)) //||this.comp==null)
        {
            Destroy();
        }
    }

    private void ResetForcedTarget()
    {
        forcedTarget = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
        if (burstCooldownTicksLeft <= 0)
        {
            TryStartShootSomething(false);
        }
    }

    private void ResetCurrentTarget()
    {
        currentTargetInt = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
    }

    public void EditGun()
    {
        var Weapon_Quality = gun.TryGetComp<CompQuality>();
        if (Weapon_Quality != null)
        {
            Comp.parent.TryGetQuality(out var Q);
            Weapon_Quality.SetQuality(Q, ArtGenerationContext.Outsider);
        }

        UpdateGunVerbs();
    }

    private void UpdateGunVerbs()
    {
        var allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
        foreach (var verb in allVerbs)
        {
            verb.caster = this;
            verb.castCompleteCallback = BurstComplete;
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
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
                verb = AttackVerb,
                hotKey = KeyBindingDefOf.Misc4
            };
            if (Spawned && IsMortarOrProjectileFliesOverhead && Position.Roofed(Map))
            {
                attack.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
            }

            yield return attack;
        }

        if (!forcedTarget.IsValid)
        {
            yield break;
        }

        var stop = new Command_Action
        {
            defaultLabel = "CommandStopForceAttack".Translate(),
            defaultDesc = "CommandStopForceAttackDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt"),
            action = delegate
            {
                ResetForcedTarget();
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            }
        };
        if (!forcedTarget.IsValid)
        {
            stop.Disable("CommandStopAttackFailNotForceAttacking".Translate());
        }

        stop.hotKey = KeyBindingDefOf.Misc5;
        yield return stop;
    }
}