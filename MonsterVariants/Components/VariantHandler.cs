using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterVariants.Components
{
    public class VariantHandler : MonoBehaviour
    {
        //TODO: sync variant spawns online

        public float spawnRate = 1f;
        public float sizeModifier = 1f;
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

        public BuffIndex buff;

        public ItemInfo[] customInventory;
        private CharacterBody body;
        private CharacterMaster master;
        private Xoroshiro128Plus rng;

        public void Init(MonsterVariantInfo variantInfo)
        {
            this.spawnRate = variantInfo.spawnRate;

            this.tier = variantInfo.variantTier;

            this.customInventory = variantInfo.customInventory;

            this.sizeModifier = variantInfo.sizeMultiplier;
            this.healthModifier = variantInfo.healthMultiplier;
            this.moveSpeedModifier = variantInfo.moveSpeedMultiplier;
            this.attackSpeedModifier = variantInfo.attackSpeedMultiplier;
            this.damageModifier = variantInfo.damageMultiplier;
            this.armorModifier = variantInfo.armorMultiplier;
            this.armorBonus = variantInfo.armorBonus;

            this.meshReplacements = variantInfo.meshReplacement;
            this.materialReplacements = variantInfo.materialReplacement;

            this.buff = variantInfo.buff;
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            }

            if (Util.CheckRoll(this.spawnRate))
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

        private void ApplyBuffs()
        {
            Debug.Log("applying buffs to " + this.gameObject.name);
            this.ScaleBody(this.sizeModifier);
            this.ModifyStats();
            this.AddItems();
            this.ModifyModel();

            // apply stat changes
            this.body.RecalculateStats();
        }

        private void ScaleBody(float modifier)
        {
            var modelLocator = this.body.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelBaseTransform;
                if (modelTransform)
                {
                    modelTransform.localScale *= modifier;

                    /*foreach (KinematicCharacterMotor kinematicCharacterMotor in this.body.GetComponentsInChildren<KinematicCharacterMotor>())
                    {
                        kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * modifier, kinematicCharacterMotor.Capsule.height * modifier, modifier);
                    }*/
                }
            }
        }
    }
}