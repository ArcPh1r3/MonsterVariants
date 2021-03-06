﻿using BepInEx;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MonsterVariants
{
    [BepInPlugin("com.rob.MonsterVariants", "MonsterVariants", "1.3.9")]

    public class MainPlugin : BaseUnityPlugin
    {
        public static MainPlugin instance;

        internal static GameObject missileLauncherDisplayPrefab; // gotta cache this for lemurians

        // fuck
        private static Material lunarGolemMat;
        private static Material goldTitanMat;
        private static Material perforatorMat;
        private static Material glandMat;
        private static Material visionsMat;
        private static Material ghostMat;
        private static Material shatterspleenMat;
        private static Material solusMat;
        private static Material flameTrailMat;
        private static Material dunestriderMat;
        private static Material blueFlameMat;
        private static Material wispFlameMat;
        private static Material greaterWispFlameMat;
        private static Material greaterWispBodyMat;
        private static Material skeltalMat;

        public void Awake()
        {
            instance = this;

            Modules.Loadouts.FixStates();
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            Modules.Skills.RegisterSkills();

            GrabMaterials();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += Init;
        }

        private void GrabMaterials()
        {
            // gather materials and meshes to use for variants
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            lunarGolemMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/LunarGolemBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
            goldTitanMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/TitanGoldBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[19].defaultMaterial);
            perforatorMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.FireballsOnHit).rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            glandMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.BeetleGland).rules[0].followerPrefab.GetComponentInChildren<Renderer>().material);
            visionsMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.LunarPrimaryReplacement).rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            ghostMat = Resources.Load<Material>("Materials/matGhostEffect");
            shatterspleenMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Items.BleedOnHitAndExplode).rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            solusMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/RoboBallBossBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
            flameTrailMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/FireMeatBallGhost").GetComponentInChildren<TrailRenderer>().material);
            dunestriderMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/ClayBossBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[2].defaultMaterial);
            blueFlameMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashLunarGolemTwinShot").transform.Find("FlameCloud_Ps").GetComponent<ParticleSystemRenderer>().material);
            wispFlameMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/WispBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[1].defaultMaterial);
            greaterWispFlameMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/GreaterWispBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[1].defaultMaterial);
            greaterWispBodyMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/GreaterWispBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
            skeltalMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BrotherDashEffect").transform.Find("Donut").GetComponent<ParticleSystemRenderer>().material);
            missileLauncherDisplayPrefab = itemDisplayRuleSet.FindDisplayRuleGroup(RoR2Content.Equipment.CommandMissile).rules[0].followerPrefab;
        }

        private void Init(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            RegisterVariants();
        }

        internal static void RegisterVariants()
        {
            Mesh beedlMesh = Modules.Assets.armoredMesh;
            //Mesh beedlSpeedMesh = Modules.Assets.speedyBeetleMesh;

            // add simple variants
            AddSimpleVariant("Beetle", Modules.Config.armoredBeetleSpawnRate.Value, MonsterVariantTier.Common, GroundSizeModifier(1.5f), 3f, 1f, 1f, 1f, 1f, 0f, 20, beedlMesh);// Armored
            AddSimpleVariant("Golem", Modules.Config.titanletSpawnRate.Value, MonsterVariantTier.Uncommon, GroundSizeModifier(2.5f), 4f, 0.5f, 1f, 3f, 1f, 0f);// Titanlet
            AddSimpleVariant("BeetleGuard", Modules.Config.beetleGuardBruteSpawnRate.Value, MonsterVariantTier.Uncommon, GroundSizeModifier(1.1f), 2f, 0.5f, 0.9f, 1.4f, 1f, 10f); // Brute
            AddSimpleVariant("Bison", Modules.Config.speedyBisonSpawnRate.Value, MonsterVariantTier.Common, null, 1f, 4f, 1f, 1f, 1f, 0f); // Speedy

            AddSimpleVariant("Titan", Modules.Config.golemletSpawnRate.Value, MonsterVariantTier.Common, GroundSizeModifier(0.3f), 1f, 5f, 1f, 1f, 1f, 0f, 0); // Golemlet
            AddSimpleVariant("Titan", Modules.Config.colossalTitanSpawnRate.Value, MonsterVariantTier.Rare, GroundSizeModifier(3f), 3f, 0.5f, 1f, 2f, 1f, 50f, 3); // Colossus

            AddSimpleVariant("Vagrant", Modules.Config.unstableVagrantSpawnRate.Value, MonsterVariantTier.Common, null, 1f, 1f, 1f, 1f, 1f, -50f); // Unstable Vagrant

            // flushed
            AddSimpleVariant("BeetleQueen2", 2, MonsterVariantTier.Common, GroundSizeModifier(0.25f), 0.8f, 5f, 1f, 1f, 1f, 0f); // Stupid


            // Generic CDR inventory
            ItemInfo[] rapidFireInventory = new ItemInfo[]    
            {
                SimpleItem("AlienHead", 20),
                SimpleItem("SecondarySkillMagazine", 20)
            };

            // fucking templars
            ItemInfo[] templarInventory = new ItemInfo[]
            {
                SimpleItem("Syringe", 10),
                SimpleItem("Hoof", 10)
            };

            // Full Shield inventory
            ItemInfo[] fullShieldInventory = SimpleInventory("ShieldOnly");

            // SPEED inventory
            ItemInfo[] speedInventory = SimpleInventory("Hoof", 5);

            // Cursed Jellyfish inventory
            //  i sincerely apologize
            ItemInfo[] hellInventory = new ItemInfo[]
            {
                SimpleItem("IceRing", 2),
                SimpleItem("SprintOutOfCombat", 3),
                SimpleItem("Crowbar", 1)
            };

            // Infernal Wisp inventory
            ItemInfo[] wispInventory = new ItemInfo[]
            {
                SimpleItem("CritGlasses", 10),
                SimpleItem("Behemoth", 3)
            };

            // Clotted Imp inventory
            ItemInfo[] clottedInventory = new ItemInfo[]
            {
                SimpleItem("UtilitySkillMagazine", 2),
                SimpleItem("CritGlasses", 4),
                SimpleItem("BleedOnHitAndExplode", 1)
            };

            // Artillery Vulture inventory
            ItemInfo[] artilleryInventory = new ItemInfo[]
            {
                SimpleItem("Clover", 3),
                SimpleItem("Missile", 1),
                SimpleItem("Behemoth", 3)
            };

            // Speedy Beetle
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Beetle",
                spawnRate = Modules.Config.speedyBeetleSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                aiModifier = MonsterVariantAIModifier.ForceSprint,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 3f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = speedInventory,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });

            // Toxic Beetle
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Beetle",
                overrideName = "Toxic Beetle",
                spawnRate = Modules.Config.toxicBeetleSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(glandMat),
                skillReplacement = PrimaryReplacement(Modules.Skills.toxicExplosionDef)
            });

            // Battle Beetle
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Beetle",
                overrideName = "BattleBeetle",
                spawnRate = Modules.Config.battleBeetleSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 2f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = PrimaryReplacement(Modules.Skills.heavyHeadbuttDef)
            });

            // Sharpshooter Beetle Guard
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "BeetleGuard",
                spawnRate = Modules.Config.beetleGuardSharpshooterSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = GroundSizeModifier(0.8f),
                healthMultiplier = 0.8f,
                moveSpeedMultiplier = 0.6f,
                attackSpeedMultiplier = 3f,
                damageMultiplier = 0.4f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = SimpleInventory("AlienHead", 10),
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });

            // Flamethrower Lemurian
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Lemurian",
                overrideName = "Flamethrower Lemurian",
                spawnRate = Modules.Config.flamethrowerLemurianSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = GroundSizeModifier(1.1f),
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 30f,
                damageMultiplier = 0.6f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = rapidFireInventory,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(perforatorMat),
                skillReplacement = null
            });

            // Badass Lemurian
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Lemurian",
                overrideName = "Badass Lemurian",
                spawnRate = Modules.Config.badassLemurianSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = PrimaryReplacement(Modules.Skills.missileLaunchDef)
            });

            // Molten Elder Lemurian
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "LemurianBruiser",
                overrideName = "Molten Elder Lemurian",
                spawnRate = Modules.Config.moltenElderLemurianSpawnRate.Value,
                variantTier = MonsterVariantTier.Uncommon,
                sizeModifier = GroundSizeModifier(1.2f),
                healthMultiplier = 0.8f,
                moveSpeedMultiplier = 0.5f,
                attackSpeedMultiplier = 0.8f,
                damageMultiplier = 1.2f,
                armorMultiplier = 1f,
                armorBonus = 500f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(flameTrailMat),
                skillReplacement = null
            });

            // Nuclear Jellyfish
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Jellyfish",
                overrideName = "Nuclear Jellyfish",
                unique = true,
                spawnRate = Modules.Config.nuclearJellyfishSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = FlyingSizeModifier(2f),
                healthMultiplier = 15f,
                moveSpeedMultiplier = 0.75f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 6f,
                armorMultiplier = 1f,
                armorBonus = 20f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(visionsMat),
                skillReplacement = SecondaryReplacement(Modules.Skills.nuclearNovaDef)
            });

            // M.O.A.J.
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Jellyfish",
                overrideName = "M.O.A.J.",
                unique = true,
                spawnRate = Modules.Config.MOAJSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = FlyingSizeModifier(4f),
                healthMultiplier = 40f,
                moveSpeedMultiplier = 0.4f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 80f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(solusMat),
                skillReplacement = SecondaryReplacement(Modules.Skills.spawnNovaDef)
            });

            // Full-Auto Golem
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Golem",
                overrideName = "Full-Auto Golem",
                spawnRate = Modules.Config.fullAutoGolemSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 8f,
                damageMultiplier = 0.8f,
                armorMultiplier = 1f,
                armorBonus = -20f,
                customInventory = rapidFireInventory,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(lunarGolemMat),
                skillReplacement = null
            });

            // Overcharged Golem
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Golem",
                overrideName = "Overcharged Golem",
                spawnRate = Modules.Config.overchargedGolemSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 3f,
                attackSpeedMultiplier = 2f,
                damageMultiplier = 10f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = fullShieldInventory,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });

            // Rush Golem
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Golem",
                overrideName = "Rush Golem",
                spawnRate = Modules.Config.rushGolemSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = GroundSizeModifier(0.6f),
                healthMultiplier = 0.9f,
                moveSpeedMultiplier = 1.6f,
                attackSpeedMultiplier = 1.6f,
                damageMultiplier = 2f,
                armorMultiplier = 1f,
                armorBonus = 100f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(goldTitanMat),
                skillReplacement = SecondaryReplacement(Modules.Skills.parentTeleportDef)
            });

            // Cursed Jellyfish
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Jellyfish",
                overrideName = "Cursed Jellyfish",
                unique = true,
                spawnRate = Modules.Config.cursedJellyfishSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = null,
                healthMultiplier = 0.8f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1.5f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = hellInventory,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(perforatorMat),
                skillReplacement = null
            });

            // Spectral Jellyfish
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Jellyfish",
                overrideName = "Spectral Jellyfish",
                unique = true,
                spawnRate = Modules.Config.spectralJellyfishSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = FlyingSizeModifier(0.8f),
                healthMultiplier = 0.5f,
                moveSpeedMultiplier = 1.5f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 0.5f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = SimpleInventory("ExtraLife"),
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(ghostMat),
                skillReplacement = null
            });

            // Infernal Wisp
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Wisp",
                overrideName = "Infernal Wisp",
                unique = true,
                spawnRate = Modules.Config.infernalWispSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = FlyingSizeModifier(1.2f),
                healthMultiplier = 1f,
                moveSpeedMultiplier = 0.8f,
                attackSpeedMultiplier = 0.6f,
                damageMultiplier = 2f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = wispInventory,
                meshReplacement = null,
                materialReplacement = MultiMaterialReplacement(new Dictionary<int, Material> { { 0, dunestriderMat }, { 1, blueFlameMat } }),
                skillReplacement = null
            });

            // Almost-But-Not-Quite-Great Wisp
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Wisp",
                overrideName = "Almost-But-Not-Quite-Great Wisp",
                unique = true,
                spawnRate = Modules.Config.almostButNotQuiteGreatWispSpawnRate.Value,
                variantTier = MonsterVariantTier.Uncommon,
                sizeModifier = FlyingSizeModifier(1.1f),
                healthMultiplier = 3f,
                moveSpeedMultiplier = 0.8f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = MultiMaterialReplacement(new Dictionary<int, Material> { { 0, greaterWispBodyMat }, { 1, greaterWispFlameMat } }),
                skillReplacement = PrimaryReplacement(Modules.Skills.wispCannonDef)
            });

            // Greatest Wisp
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "GreaterWisp",
                overrideName = "Greatest Wisp",
                spawnRate = Modules.Config.greatestWispSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = FlyingSizeModifier(1.2f),
                healthMultiplier = 2f,
                moveSpeedMultiplier = 5f,
                attackSpeedMultiplier = 2f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = SimpleInventory("AlienHead", 5),
                meshReplacement = null,
                materialReplacement = MultiMaterialReplacement(new Dictionary<int, Material> { { 0, flameTrailMat }, { 1, wispFlameMat } }),
                skillReplacement = null
            });

            // Albino Bison
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Bison",
                overrideName = "Albino Bison",
                spawnRate = Modules.Config.albinoBisonSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = GroundSizeModifier(1.2f),
                healthMultiplier = 2f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1.5f,
                armorMultiplier = 1f,
                armorBonus = 50f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = MultiMaterialReplacement(new Dictionary<int, Material> { { 0, skeltalMat }, { 1, null } }),
                skillReplacement = null
            });

            // Clotted Imp
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Imp",
                overrideName = "Clotted Imp",
                spawnRate = Modules.Config.clottedImpSpawnRate.Value,
                variantTier = MonsterVariantTier.Uncommon,
                sizeModifier = GroundSizeModifier(0.8f),
                healthMultiplier = 0.8f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = clottedInventory,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(shatterspleenMat),
                skillReplacement = null
            });

            // Artillery Vulture
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Vulture",
                overrideName = "Artillery Vulture",
                spawnRate = Modules.Config.artilleryVultureSpawnRate.Value,
                variantTier = MonsterVariantTier.Uncommon,
                sizeModifier = GroundSizeModifier(1.25f),
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 4f,
                damageMultiplier = 0.5f,
                armorMultiplier = 1f,
                armorBonus = 100f,
                customInventory = artilleryInventory,
                meshReplacement = null,
                materialReplacement = MultiMaterialReplacement(new Dictionary<int, Material> { { 0, perforatorMat }, { 2, solusMat } }),
                skillReplacement = null
            });

            // Gun Vulture
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Vulture",
                spawnRate = Modules.Config.gunVultureSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 0.25f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = PrimaryReplacement(Modules.Skills.doubleTapDef)
            });

            // Fucking Templar
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "ClayBruiser",
                overrideName = "Fucking Templar",
                spawnRate = Modules.Config.fuckingTemplarSpawnRate.Value,
                variantTier = MonsterVariantTier.Uncommon,
                sizeModifier = GroundSizeModifier(1.2f),
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 0.25f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = templarInventory,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });

            // Dream Scavenger
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Scav",
                overrideName = "Dream",
                spawnRate = Modules.Config.dreamScavSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeModifier = null,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = UtilityReplacement(Modules.Skills.dreamLuckDef)
            });
        }

        // helper for simplifying mat replacements
        public static MonsterMaterialReplacement[] SimpleMaterialReplacement(Material newMaterial)
        {
            return SimpleMaterialReplacement(newMaterial, 0);
        }

        public static MonsterMaterialReplacement[] SimpleMaterialReplacement(Material newMaterial, int index)
        {
            MonsterMaterialReplacement replacement = ScriptableObject.CreateInstance<MonsterMaterialReplacement>();
            replacement.rendererIndex = index;
            replacement.material = newMaterial;

            MonsterMaterialReplacement[] matReplacement = new MonsterMaterialReplacement[]
            {
                replacement
            };

            return matReplacement;
        }

        public static MonsterMaterialReplacement SingleMaterialReplacement(Material newMaterial, int index)
        {
            MonsterMaterialReplacement replacement = ScriptableObject.CreateInstance<MonsterMaterialReplacement>();
            replacement.rendererIndex = index;
            replacement.material = newMaterial;

            return replacement;
        }

        // helper for multiple material replacement, takes a dictionary with <renderer,material>
        public static MonsterMaterialReplacement[] MultiMaterialReplacement(Dictionary<int, Material> newMaterials)
        {
            List<MonsterMaterialReplacement> matReplacement = new List<MonsterMaterialReplacement>();
            foreach (KeyValuePair<int, Material> kvp in newMaterials)
            {
                MonsterMaterialReplacement replacement = SingleMaterialReplacement(kvp.Value, kvp.Key);
                matReplacement.Add(replacement);
            }

            return matReplacement.ToArray();
        }

        // helper for simplifying mesh replacements
        public static MonsterMeshReplacement[] SimpleMeshReplacement(Mesh newMesh)
        {
            return SimpleMeshReplacement(newMesh, 0);
        }

        public static MonsterMeshReplacement[] SimpleMeshReplacement(Mesh newMesh, int index)
        {
            MonsterMeshReplacement replacement = ScriptableObject.CreateInstance<MonsterMeshReplacement>();
            replacement.rendererIndex = index;
            replacement.mesh = newMesh;

            MonsterMeshReplacement[] meshReplacement = new MonsterMeshReplacement[]
            {
                replacement
            };

            return meshReplacement;
        }

        public static MonsterMeshReplacement SingleMeshReplacement(Mesh newMesh, int index)
        {
            MonsterMeshReplacement replacement = ScriptableObject.CreateInstance<MonsterMeshReplacement>();
            replacement.rendererIndex = index;
            replacement.mesh = newMesh;

            return replacement;
        }

        // helpers for adding simple variants
        public static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, MonsterSizeModifier size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, 0, null, null);
        }

        public static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, MonsterSizeModifier size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, null, null);
        }

        public static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, MonsterSizeModifier size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Material replacementMaterial)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, replacementMaterial, null);
        }

        public static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, MonsterSizeModifier size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Mesh replacementMesh)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, null, replacementMesh);
        }

        public static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, MonsterSizeModifier size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Material replacementMaterial, Mesh replacementMesh)
        {
            MonsterMaterialReplacement[] replacementMats = null;
            if (replacementMaterial != null) replacementMats = SimpleMaterialReplacement(replacementMaterial);

            MonsterMeshReplacement[] replacementMeshes = null;
            if (replacementMesh != null) replacementMeshes = SimpleMeshReplacement(replacementMesh);

            MonsterVariantInfo newInfo = new MonsterVariantInfo
            {
                bodyName = bodyName,
                spawnRate = spawnRate,
                variantTier = tier,
                sizeModifier = size,
                healthMultiplier = health,
                moveSpeedMultiplier = moveSpeed,
                attackSpeedMultiplier = attackSpeed,
                damageMultiplier = damage,
                armorMultiplier = armor,
                armorBonus = armorBonus,
                customInventory = SimpleInventory("AlienHead", alienHeads),
                meshReplacement = replacementMeshes,
                materialReplacement = replacementMats,
                skillReplacement = null,
                buff = BuffIndex.None
            };

            Components.VariantHandler variantHandler = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").AddComponent<Components.VariantHandler>();
            variantHandler.Init(newInfo);
        }

        // helper to simplify adding a variant
        public static void AddVariant(MonsterVariantInfo info)
        {
            AddVariant(Resources.Load<GameObject>("Prefabs/CharacterBodies/" + info.bodyName + "Body"), info);
        }

        public static void AddVariant(GameObject bodyPrefab, MonsterVariantInfo info)
        {
            if (!bodyPrefab)
            {
                Debug.LogError("Failed to add variant: Invalid body prefab");
                return;
            }

            Components.VariantHandler variantHandler = bodyPrefab.AddComponent<Components.VariantHandler>();
            variantHandler.Init(info);
        }

        public static void AddModdedVariant(MonsterVariantInfo info)
        {
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();

                GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(info.bodyName + "Body");
                if (!bodyPrefab)
                {
                    Debug.LogError("Failed to add variant: " + info.bodyName + "Body does not exist.");
                    return;
                }

                bodyPrefab.AddComponent<Components.VariantHandler>().Init(info);
            };
        }

        // helper to simplify creating an inventory with one item
        public static ItemInfo[] SimpleInventory(string itemName)
        {
            return SimpleInventory(itemName, 1);
        }

        public static ItemInfo[] SimpleInventory(string itemName, int itemCount)
        {
            ItemInfo info = SimpleItem(itemName, itemCount);

            List<ItemInfo> infos = new List<ItemInfo>();

            infos.Add(info);

            ItemInfo[] newInfos = infos.ToArray();

            return newInfos;
        }

        public static ItemInfo SimpleItem(string itemName)
        {
            return SimpleItem(itemName, 1);
        }

        public static ItemInfo SimpleItem(string itemName, int itemCount)
        {
            ItemInfo info = ScriptableObject.CreateInstance<ItemInfo>();
            info.itemString = itemName;
            info.count = itemCount;

            return info;
        }

        // all these aren't needed but it's nice to keep things clean up there
        public static MonsterSkillReplacement[] PrimaryReplacement(SkillDef skill)
        {
            MonsterSkillReplacement skillReplacement = ScriptableObject.CreateInstance<MonsterSkillReplacement>();
            skillReplacement.skillSlot = SkillSlot.Primary;
            skillReplacement.skillDef = skill;

            return new MonsterSkillReplacement[]
            {
                skillReplacement
            };
        }

        public static MonsterSkillReplacement[] SecondaryReplacement(SkillDef skill)
        {
            MonsterSkillReplacement skillReplacement = ScriptableObject.CreateInstance<MonsterSkillReplacement>();
            skillReplacement.skillSlot = SkillSlot.Secondary;
            skillReplacement.skillDef = skill;

            return new MonsterSkillReplacement[]
            {
                skillReplacement
            };
        }

        public static MonsterSkillReplacement[] UtilityReplacement(SkillDef skill)
        {
            MonsterSkillReplacement skillReplacement = ScriptableObject.CreateInstance<MonsterSkillReplacement>();
            skillReplacement.skillSlot = SkillSlot.Utility;
            skillReplacement.skillDef = skill;

            return new MonsterSkillReplacement[]
            {
                skillReplacement
            };
        }

        public static MonsterSkillReplacement[] SpecialReplacement(SkillDef skill)
        {
            MonsterSkillReplacement skillReplacement = ScriptableObject.CreateInstance<MonsterSkillReplacement>();
            skillReplacement.skillSlot = SkillSlot.Special;
            skillReplacement.skillDef = skill;

            return new MonsterSkillReplacement[]
            {
                skillReplacement
            };
        }

        // eh don't need these but same as above
        public static MonsterSizeModifier GroundSizeModifier(float newSize)
        {
            MonsterSizeModifier sizeModifier = ScriptableObject.CreateInstance<MonsterSizeModifier>();
            sizeModifier.newSize = newSize;
            sizeModifier.scaleCollider = false;

            return sizeModifier;
        }

        public static MonsterSizeModifier FlyingSizeModifier(float newSize)
        {
            MonsterSizeModifier sizeModifier = ScriptableObject.CreateInstance<MonsterSizeModifier>();
            sizeModifier.newSize = newSize;
            sizeModifier.scaleCollider = true;

            return sizeModifier;
        }
    }
}