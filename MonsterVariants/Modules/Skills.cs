using EntityStates;
using MonsterVariants.Modules.States;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterVariants.Modules
{
    internal static class Skills
    {
        internal static SkillDef missileLaunchDef;

        internal static SkillDef nuclearNovaDef;
        internal static SkillDef spawnNovaDef;

        internal static SkillDef doubleTapDef;

        internal static SkillDef toxicExplosionDef;
        internal static SkillDef heavyHeadbuttDef;

        internal static SkillDef parentTeleportDef;

        internal static SkillDef dreamLuckDef;

        internal static SkillDef wispCannonDef;

        internal static void RegisterSkills()
        {
            LoadoutAPI.AddSkill(typeof(LaunchMissile));
            LoadoutAPI.AddSkill(typeof(NuclearNova));
            LoadoutAPI.AddSkill(typeof(SpawnNova));
            LoadoutAPI.AddSkill(typeof(ExplosiveHeadbutt));
            LoadoutAPI.AddSkill(typeof(LaunchingHeadbutt));
            LoadoutAPI.AddSkill(typeof(ChargeWispCannon));
            LoadoutAPI.AddSkill(typeof(ParentWarp));
            LoadoutAPI.AddSkill(typeof(EnterLuckySit));
            LoadoutAPI.AddSkill(typeof(DreamLuck));

            missileLaunchDef = NewSkillDef(new SerializableEntityStateType(typeof(LaunchMissile)), "Weapon");
            nuclearNovaDef = NewSkillDef(new SerializableEntityStateType(typeof(NuclearNova)), "Body");
            spawnNovaDef = NewSkillDef(new SerializableEntityStateType(typeof(SpawnNova)), "Body");
            toxicExplosionDef = NewSkillDef(new SerializableEntityStateType(typeof(ExplosiveHeadbutt)), "Body");
            heavyHeadbuttDef = NewSkillDef(new SerializableEntityStateType(typeof(LaunchingHeadbutt)), "Body");
            wispCannonDef = NewSkillDef(new SerializableEntityStateType(typeof(ChargeWispCannon)), "Weapon");
            parentTeleportDef = NewSkillDef(new SerializableEntityStateType(typeof(ParentWarp)), "Body");

            parentTeleportDef.baseMaxStock = 2;
            parentTeleportDef.baseRechargeInterval = 8f;
            parentTeleportDef.requiredStock = 1;
            parentTeleportDef.stockToConsume = 1;

            dreamLuckDef = NewSkillDef(new SerializableEntityStateType(typeof(EnterLuckySit)), "Body");
            dreamLuckDef.baseMaxStock = 1;
            dreamLuckDef.baseRechargeInterval = 24f;
            dreamLuckDef.requiredStock = 1;
            dreamLuckDef.stockToConsume = 1;

            doubleTapDef = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef;
        }

        private static SkillDef NewSkillDef(SerializableEntityStateType state, string stateMachine)
        {
            return NewSkillDef(state, stateMachine, 1, 0, 0);
        }

        private static SkillDef NewSkillDef(SerializableEntityStateType state, string stateMachine, int stock, float cooldown, int stockConsumed)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = "SKILL_LUNAR_PRIMARY_REPLACEMENT_NAME";
            skillDef.skillNameToken = "SKILL_LUNAR_PRIMARY_REPLACEMENT_NAME";
            skillDef.skillDescriptionToken = "SKILL_LUNAR_PRIMARY_REPLACEMENT_DESCRIPTION";
            skillDef.icon = null;

            skillDef.activationState = state;
            skillDef.activationStateMachineName = stateMachine;
            skillDef.baseMaxStock = stock;
            skillDef.baseRechargeInterval = cooldown;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.forceSprintDuringState = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Any;
            skillDef.isBullets = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.noSprint = true;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.shootDelay = 0f;
            skillDef.stockToConsume = stockConsumed;

            LoadoutAPI.AddSkillDef(skillDef);

            return skillDef;
        }
    }
}
