using BepInEx;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterVariants
{
    [BepInPlugin("com.rob.MonsterVariants", "MonsterVariants", "1.1.0")]

    public class MainPlugin : BaseUnityPlugin
    {
        public static MainPlugin instance;

        public void Awake()
        {
            instance = this;

            // load assets and config before doing anything
            //  currently not loading any assets so no point running this
            //Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();

            RegisterVariants();
        }

        internal static void RegisterVariants()
        {
            // gather materials and meshes to use for variants
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            Material lunarGolemMat = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterBodies/LunarGolemBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
            Material perforatorMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindItemDisplayRuleGroup("FireballsOnHit").rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            //Material glandMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindItemDisplayRuleGroup("BeetleGland").rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            Material visionsMat = UnityEngine.Object.Instantiate(itemDisplayRuleSet.FindItemDisplayRuleGroup("LunarPrimaryReplacement").rules[0].followerPrefab.GetComponentInChildren<MeshRenderer>().material);
            Material ghostMat = Resources.Load<Material>("Materials/matGhostEffect");

            //Mesh beedlMesh = Modules.Assets.armoredMesh;
            //Mesh beedlSpeedMesh = Modules.Assets.speedyBeetleMesh;

            // add simple variants
            AddSimpleVariant("Beetle", Modules.Config.armoredBeetleSpawnRate.Value, MonsterVariantTier.Common, 1.35f, 3f, 1f, 1f, 1f, 1f, 0f, 20);// Armored
            AddSimpleVariant("Golem", Modules.Config.fullAutoGolemSpawnRate.Value, MonsterVariantTier.Rare, 1f, 1f, 1f, 8f, 0.8f, 1f, -20f, 20, lunarGolemMat);// Full-Auto
            AddSimpleVariant("Golem", Modules.Config.titanletSpawnRate.Value, MonsterVariantTier.Uncommon, 2.5f, 4f, 0.5f, 1f, 3f, 1f, 0f);// Titanlet
            AddSimpleVariant("Jellyfish", Modules.Config.nuclearJellyfishSpawnRate.Value, MonsterVariantTier.Common, 2.5f, 20f, 0.75f, 0.75f, 6f, 1f, 50f, 0, visionsMat);// Nuclear
            AddSimpleVariant("BeetleGuard", Modules.Config.beetleGuardBruteSpawnRate.Value, MonsterVariantTier.Uncommon, 1.1f, 2f, 0.5f, 0.9f, 1.4f, 1f, 10f); // Brute
            //AddSimpleVariant("Bison", 50, MonsterVariantTier.Uncommon, 1f, 1f, 4f, 1f, 1f, 1f, 1f); // Speedy

            // Generic CDR inventory
            ItemInfo[] rapidFireInventory = SimpleInventory("AlienHead", 20);

            // Full Shield inventory
            ItemInfo[] fullShieldInventory = SimpleInventory("ShieldOnly");

            // SPEED inventory
            ItemInfo[] speedInventory = SimpleInventory("Hoof", 5);

            // Jellyfish inventory
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
                new ItemInfo
                {
                    itemString = "CritGlasses",
                    count = 10
                },
                new ItemInfo
                {
                    itemString = "Behemoth",
                    count = 3
                }
            };

            // Speedy Beetle
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Beetle",
                spawnRate = Modules.Config.speedyBeetleSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeMultiplier = 1f,
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

            // Flamethrower Lemurian
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Lemurian",
                spawnRate = Modules.Config.flamethrowerLemurianSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeMultiplier = 1.1f,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 30f,
                damageMultiplier = 0.4f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = rapidFireInventory,
                meshReplacement = null,
                materialReplacement = SimpleMaterialReplacement(perforatorMat),
                skillReplacement = null
            });

            // Overcharged Golem
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Golem",
                spawnRate = Modules.Config.overchargedGolemSpawnRate.Value,
                variantTier = MonsterVariantTier.Common,
                sizeMultiplier = 1f,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 1f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = fullShieldInventory,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });

            // Cursed Jellyfish
            AddVariant(new MonsterVariantInfo
            {
                bodyName = "Jellyfish",
                spawnRate = Modules.Config.cursedJellyfishSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeMultiplier = 1f,
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
                spawnRate = Modules.Config.spectralJellyfishSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeMultiplier = 0.9f,
                healthMultiplier = 0.5f,
                moveSpeedMultiplier = 0.8f,
                attackSpeedMultiplier = 1f,
                damageMultiplier = 1f,
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
                spawnRate = Modules.Config.infernalWispSpawnRate.Value,
                variantTier = MonsterVariantTier.Rare,
                sizeMultiplier = 1.1f,
                healthMultiplier = 1f,
                moveSpeedMultiplier = 0.8f,
                attackSpeedMultiplier = 0.6f,
                damageMultiplier = 2f,
                armorMultiplier = 1f,
                armorBonus = 0f,
                customInventory = wispInventory,
                meshReplacement = null,
                materialReplacement = null,
                skillReplacement = null
            });
        }
       
        // helper for simplifying mat replacements
        public static MonsterMaterialReplacement[] SimpleMaterialReplacement(Material newMaterial)
        {
            MonsterMaterialReplacement replacement = ScriptableObject.CreateInstance<MonsterMaterialReplacement>();
            replacement.rendererIndex = 0;
            replacement.material = newMaterial;

            MonsterMaterialReplacement[] matReplacement = new MonsterMaterialReplacement[]
            {
                replacement
            };

            return matReplacement;
        }

        // helpers for adding simple variants
        internal static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, float size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, 0, null, null);
        }

        internal static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, float size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, null, null);
        }

        internal static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, float size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Material replacementMaterial)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, replacementMaterial, null);
        }

        internal static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, float size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Mesh replacementMesh)
        {
            AddSimpleVariant(bodyName, spawnRate, tier, size, health, moveSpeed, attackSpeed, damage, armor, armorBonus, alienHeads, null, replacementMesh);
        }

        internal static void AddSimpleVariant(string bodyName, float spawnRate, MonsterVariantTier tier, float size, float health, float moveSpeed, float attackSpeed, float damage, float armor, float armorBonus, int alienHeads, Material replacementMaterial, Mesh replacementMesh)
        {
            MonsterMaterialReplacement[] replacementMats = null;
            if (replacementMaterial != null)
            {
                replacementMats = new MonsterMaterialReplacement[]
                {
                    new MonsterMaterialReplacement
                    {
                        material = replacementMaterial,
                        rendererIndex = 0
                    }
                };
            }

            MonsterVariantInfo newInfo = new MonsterVariantInfo
            {
                bodyName = bodyName,
                spawnRate = spawnRate,
                variantTier = tier,
                sizeMultiplier = size,
                healthMultiplier = health,
                moveSpeedMultiplier = moveSpeed,
                attackSpeedMultiplier = attackSpeed,
                damageMultiplier = damage,
                armorMultiplier = armor,
                armorBonus = armorBonus,
                customInventory = null,
                meshReplacement = null,
                materialReplacement = replacementMats,
                skillReplacement = null,
                buff = BuffIndex.None
            };

            Components.VariantHandler variantHandler = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").AddComponent<Components.VariantHandler>();
            variantHandler.Init(newInfo);
        }

        // helper to simplify adding a variant
        internal static void AddVariant(MonsterVariantInfo info)
        {
            Components.VariantHandler variantHandler = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + info.bodyName + "Body").AddComponent<Components.VariantHandler>();
            variantHandler.Init(info);
        }

        // helper to simplify creating an inventory with one item
        internal static ItemInfo[] SimpleInventory(string itemName)
        {
            return SimpleInventory(itemName, 1);
        }

        internal static ItemInfo[] SimpleInventory(string itemName, int itemCount)
        {
            ItemInfo info = SimpleItem(itemName, itemCount);

            ItemInfo[] newInfos = new ItemInfo[]
            {
                info
            };

            return newInfos;
        }

        internal static ItemInfo SimpleItem(string itemName)
        {
            return SimpleItem(itemName, 1);
        }

        internal static ItemInfo SimpleItem(string itemName, int itemCount)
        {
            ItemInfo info = ScriptableObject.CreateInstance<ItemInfo>();
            info.itemString = itemName;
            info.count = itemCount;

            return info;
        }
    }
}