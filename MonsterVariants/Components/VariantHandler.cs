using KinematicCharacterController;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterVariants.Components
{
    public class VariantHandler : NetworkBehaviour
    {
        //might be synced now?

        [SyncVar]
        public bool isVariant = false;

        public float spawnRate = 1f;
        public float healthModifier = 1f;
        public float moveSpeedModifier = 1f;
        public float attackSpeedModifier = 1f;
        public float damageModifier = 1f;
        public float armorModifier = 1f;
        public float armorBonus = 0f;

        public MonsterVariantTier tier;

        public MonsterMaterialReplacement[] materialReplacements;
        public MonsterMeshReplacement[] meshReplacements;
        public MonsterSkillReplacement[] skillReplacements;
        public MonsterSizeModifier sizeModifier;

        public BuffIndex buff;

        public ItemInfo[] customInventory;
        private CharacterBody body;
        private CharacterMaster master;

        public void Init(MonsterVariantInfo variantInfo)
        {
            this.spawnRate = variantInfo.spawnRate;

            this.tier = variantInfo.variantTier;

            this.customInventory = variantInfo.customInventory;

            this.healthModifier = variantInfo.healthMultiplier;
            this.moveSpeedModifier = variantInfo.moveSpeedMultiplier;
            this.attackSpeedModifier = variantInfo.attackSpeedMultiplier;
            this.damageModifier = variantInfo.damageMultiplier;
            this.armorModifier = variantInfo.armorMultiplier;
            this.armorBonus = variantInfo.armorBonus;

            this.meshReplacements = variantInfo.meshReplacement;
            this.materialReplacements = variantInfo.materialReplacement;
            this.skillReplacements = variantInfo.skillReplacement;
            this.sizeModifier = variantInfo.sizeModifier;

            this.buff = variantInfo.buff;
        }

        private void Awake()
        {
            if (!NetworkServer.active) return;

            if (Util.CheckRoll(this.spawnRate)) this.isVariant = true;
        }

        private void Start()
        {
            if (this.isVariant)
            {
                this.body = base.GetComponent<CharacterBody>();
                if (this.body)
                {
                    this.master = this.body.master;

                    if (this.master)
                    {
                        this.ApplyBuffs();
                    }
                }
            }
        }

        private void AddItems()
        {
            if (this.master.inventory)
            {
                if (this.customInventory == null) this.customInventory = new ItemInfo[0];
                // add items from the set inventory
                if (this.customInventory.Length > 0)
                {
                    for (int i = 0; i < this.customInventory.Length; i++)
                    {
                        bool giveItem = true;
                        if (this.customInventory[i].itemString == "ExtraLife")
                        {
                            if (this.master.GetComponent<PreventJellyfishRecursion>()) giveItem = false;
                            else this.master.gameObject.AddComponent<PreventJellyfishRecursion>();
                        }

                        if (giveItem) this.master.inventory.GiveItemString(this.customInventory[i].itemString, this.customInventory[i].count);
                    }
                }

                // add an infusion as a cheap way to turn health bars red, lol
                if (this.tier == MonsterVariantTier.Uncommon || this.tier == MonsterVariantTier.Rare)
                {
                    this.master.inventory.GiveItem(ItemIndex.Infusion);
                }
            }
        }

        private void ModifyStats()
        {
            this.body.baseMaxHealth *= this.healthModifier;
            this.body.baseMoveSpeed *= this.moveSpeedModifier;
            this.body.baseAttackSpeed *= this.attackSpeedModifier;
            this.body.baseDamage *= this.damageModifier;
            this.body.levelDamage = this.body.baseDamage * 0.2f;
            this.body.baseArmor *= this.armorModifier;
            this.body.baseArmor += this.armorBonus;
        }

        private void ModifyModel()
        {
            // get model
            CharacterModel model = null;
            ModelLocator modelLocator = this.body.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelTransform;
                if (modelTransform) model = modelTransform.GetComponent<CharacterModel>();
            }

            if (model)
            {
                if (this.materialReplacements == null) this.materialReplacements = new MonsterMaterialReplacement[0];
                if (this.meshReplacements == null) this.meshReplacements = new MonsterMeshReplacement[0];

                // replace materials
                if (this.materialReplacements.Length > 0)
                {
                    for (int i = 0; i < this.materialReplacements.Length; i++)
                    {
                        model.baseRendererInfos[this.materialReplacements[i].rendererIndex].defaultMaterial = this.materialReplacements[i].material;
                    }
                }

                // replace meshes
                if (this.meshReplacements.Length > 0)
                {
                    for (int i = 0; i < this.meshReplacements.Length; i++)
                    {
                        model.baseRendererInfos[this.meshReplacements[i].rendererIndex].renderer.GetComponent<SkinnedMeshRenderer>().sharedMesh = this.meshReplacements[i].mesh;
                    }
                }
            }
        }

        private void SwapSkills()
        {
            if (this.skillReplacements == null) return;

            SkillLocator skillLocator = this.body.skillLocator;

            if (skillLocator)
            {
                for (int i = 0; i < skillReplacements.Length; i++)
                {
                    switch (skillReplacements[i].skillSlot)
                    {
                        case SkillSlot.Primary:
                            skillLocator.primary.SetSkillOverride(this.gameObject, skillReplacements[i].skillDef, GenericSkill.SkillOverridePriority.Upgrade);
                            break;
                        case SkillSlot.Secondary:
                            skillLocator.secondary.SetSkillOverride(this.gameObject, skillReplacements[i].skillDef, GenericSkill.SkillOverridePriority.Upgrade);
                            break;
                        case SkillSlot.Utility:
                            skillLocator.utility.SetSkillOverride(this.gameObject, skillReplacements[i].skillDef, GenericSkill.SkillOverridePriority.Upgrade);
                            break;
                        case SkillSlot.Special:
                            skillLocator.special.SetSkillOverride(this.gameObject, skillReplacements[i].skillDef, GenericSkill.SkillOverridePriority.Upgrade);
                            break;
                        case SkillSlot.None:
                            //what are you actually trying to do here??
                            break;
                    }

                    // gotta add the missile launcher lmao- maybe a better system for this one day
                    if (this.skillReplacements[i].skillDef == Modules.Skills.missileLaunchDef)
                    {
                        ModelLocator modelLocator = this.body.GetComponent<ModelLocator>();
                        if (modelLocator)
                        {
                            Transform modelTransform = modelLocator.modelTransform;
                            if (modelTransform) modelTransform.gameObject.AddComponent<AddMissileLauncherToLemurian>();
                        }
                    }
                }
            }
        }

        private void ApplyBuffs()
        {
            this.ModifyStats();
            this.AddItems();
            this.ModifyModel();
            this.SwapSkills();

            // apply stat changes
            this.body.RecalculateStats();

            this.ScaleBody();

            this.body.healthComponent.health = this.body.healthComponent.fullHealth;
        }

        private void ScaleBody()
        {
            if (this.sizeModifier == null) return;

            ModelLocator modelLocator = this.body.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelBaseTransform;
                if (modelTransform)
                {
                    modelTransform.localScale *= this.sizeModifier.newSize;

                    if (this.sizeModifier.scaleCollider)
                    {
                        foreach (KinematicCharacterMotor kinematicCharacterMotor in this.body.GetComponentsInChildren<KinematicCharacterMotor>())
                        {
                            if (kinematicCharacterMotor) kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * this.sizeModifier.newSize, kinematicCharacterMotor.Capsule.height * this.sizeModifier.newSize, this.sizeModifier.newSize);
                        }
                    }
                }
            }
        }
    }
}