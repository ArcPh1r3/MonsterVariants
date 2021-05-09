using KinematicCharacterController;
using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterVariants.Components
{
    public class VariantHandler : NetworkBehaviour
    {
        [SyncVar]
        public bool isVariant = false;

        public bool unique;

        public string overrideName;

        public float spawnRate = 1f;
        public float healthModifier = 1f;
        public float moveSpeedModifier = 1f;
        public float attackSpeedModifier = 1f;
        public float damageModifier = 1f;
        public float armorModifier = 1f;
        public float armorBonus = 0f;

        public MonsterVariantTier tier;
        public MonsterVariantAIModifier aiModifier;

        public MonsterMaterialReplacement[] materialReplacements;
        public MonsterMeshReplacement[] meshReplacements;
        public MonsterSkillReplacement[] skillReplacements;
        public MonsterSizeModifier sizeModifier;

        public BuffIndex buff;

        public ItemInfo[] customInventory;
        private CharacterBody body;
        private CharacterMaster master;
        private EquipmentIndex storedEquipment;
        private ItemDisplayRuleSet storedIDRS;

        public void Init(MonsterVariantInfo variantInfo)
        {
            this.overrideName = variantInfo.overrideName;

            this.spawnRate = variantInfo.spawnRate;
            this.unique = variantInfo.unique;

            this.tier = variantInfo.variantTier;
            this.aiModifier = variantInfo.aiModifier;

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

            if (this.meshReplacements != null && this.isVariant)
            {
                if (this.meshReplacements.Length > 0)
                {
                    this.storedIDRS = this.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;
                    this.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet = null;
                }
            }
        }

        private void Start()
        {
            if (this.isVariant)
            {
                if (this.unique)
                {
                    foreach (VariantHandler i in this.GetComponents<VariantHandler>())
                    {
                        if (i && i != this) Destroy(i);
                    }
                }

                this.body = base.GetComponent<CharacterBody>();
                if (this.body)
                {
                    this.master = this.body.master;

                    if (this.master)
                    {
                        this.ApplyBuffs();

                        if (this.aiModifier.HasFlag(MonsterVariantAIModifier.Unstable))
                        {
                            foreach (AISkillDriver i in this.master.GetComponents<AISkillDriver>())
                            {
                                if (i)
                                {
                                    i.minTargetHealthFraction = Mathf.NegativeInfinity;
                                    i.maxTargetHealthFraction = Mathf.Infinity;
                                    i.minUserHealthFraction = Mathf.NegativeInfinity;
                                    i.maxUserHealthFraction = Mathf.Infinity;
                                }
                            }
                        }

                        if (this.aiModifier.HasFlag(MonsterVariantAIModifier.ForceSprint))
                        {
                            foreach (AISkillDriver i in this.master.GetComponents<AISkillDriver>())
                            {
                                if (i)
                                {
                                    i.shouldSprint = true;
                                }
                            }
                        }

                        if (this.name == "VagrantBody(Clone)")
                        {
                            // i really should not be hard coding this
                            foreach(AISkillDriver i in this.master.GetComponents<AISkillDriver>())
                            {
                                if (i)
                                {
                                    i.minTargetHealthFraction = Mathf.NegativeInfinity;
                                    i.maxTargetHealthFraction = Mathf.Infinity;
                                    i.minUserHealthFraction = Mathf.NegativeInfinity;
                                    i.maxUserHealthFraction = Mathf.Infinity;
                                }
                            }
                        }
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
                    this.master.inventory.GiveItem(RoR2Content.Items.Infusion);
                }
            }
        }

        private void ModifyStats()
        {
            if (this.overrideName != "") this.body.baseNameToken = this.overrideName;

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

                    if (body.name == "GolemBody(Clone)")
                    {
                        model.baseLightInfos[0].defaultColor = Color.blue;
                    }
                }

                // replace meshes
                if (this.meshReplacements.Length > 0)
                {
                    // only run this method on beetles- don't bother changing it for now since there's no other mesh replacements
                    this.FuckWithBoneStructure();

                    for (int i = 0; i < this.meshReplacements.Length; i++)
                    {
                        model.baseRendererInfos[this.meshReplacements[i].rendererIndex].renderer.GetComponent<SkinnedMeshRenderer>().sharedMesh = this.meshReplacements[i].mesh;
                    }
                }
            }
        }

        private void RestoreEquipment()
        {
            CharacterModel model = null;
            ModelLocator modelLocator = this.body.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelTransform;
                if (modelTransform) model = modelTransform.GetComponent<CharacterModel>();
            }

            if (model && this.storedIDRS != null) model.itemDisplayRuleSet = this.storedIDRS;

            this.master.inventory.SetEquipmentIndex(this.storedEquipment);
        }

        private void FuckWithBoneStructure()
        {
            this.storedEquipment = this.master.inventory.GetEquipmentIndex();
            this.master.inventory.SetEquipmentIndex(EquipmentIndex.None);
            this.Invoke("RestoreEquipment", 0.2f);

            List<Transform> t = new List<Transform>();

            foreach (var item in this.body.GetComponentsInChildren<Transform>())
            {
                if (!item.name.Contains("Hurtbox") && !item.name.Contains("BeetleBody") && !item.name.Contains("Mesh") && !item.name.Contains("mdl"))
                {
                    t.Add(item);
                }
            }

            Transform temp = t[14];
            t[14] = t[11];
            t[11] = temp;
            temp = t[15];
            t[15] = t[12];
            t[12] = temp;
            temp = t[16];
            t[16] = t[13];
            t[13] = temp;

            foreach (var item in this.body.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                item.bones = t.ToArray();
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

                    // aaaaand hardcoding more bullshit
                    if (this.skillReplacements[i].skillDef == Modules.Skills.doubleTapDef)
                    {
                        ModelLocator modelLocator = this.body.GetComponent<ModelLocator>();
                        if (modelLocator)
                        {
                            Transform modelTransform = modelLocator.modelTransform;
                            if (modelTransform) modelTransform.gameObject.AddComponent<AddGunToVulture>();
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