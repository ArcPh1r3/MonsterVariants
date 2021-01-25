using System;
using EntityStates;
using EntityStates.LemurianBruiserMonster;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace MonsterVariants.Modules.States
{
    public class LaunchMissile : BaseSkillState
    {
        public static float damageCoefficient = 1.2f;
        public static float baseDuration = 1.75f;
        public static float recoil = 1f;
        public static GameObject projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = LaunchMissile.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            this.muzzleString = "Chest";
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireMissile()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                base.characterBody.AddSpreadBloom(LaunchMissile.recoil);

                Ray aimRay = base.GetAimRay();
                EffectManager.SimpleMuzzleFlash(FireMegaFireball.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(LaunchMissile.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(Vector3.up), base.gameObject, LaunchMissile.damageCoefficient * this.damageStat, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                this.FireMissile();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}