using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterVariants
{
    // for storing variant data
    public class MonsterVariantInfo
    {
        // name of the body to apply this variant info to
        public string bodyName;

        // new name of the variant
        public string overrideName;

        // eh
        public bool unique;

        // ai modifications
        public MonsterVariantAIModifier aiModifier;

        // spawn rate of the variant: 0 - 100
        public float spawnRate;

        // tier of the variant- uncommons have red health bar and rares have a unique spawn sound
        public MonsterVariantTier variantTier;

        // inventory the variant spawns with
        public ItemInfo[] customInventory;

        // replacing visuals- just leave them null unless you know how to grab materials from other ingame sources or build assetbundles
        public MonsterMeshReplacement[] meshReplacement;
        public MonsterMaterialReplacement[] materialReplacement;

        // replacing skills- also leave null unless you know what you're doing
        public MonsterSkillReplacement[] skillReplacement;

        // size multiplier
        public MonsterSizeModifier sizeModifier;

        // buff to spawn monster with
        public BuffIndex buff;

        // base stats
        public float healthMultiplier;
        public float moveSpeedMultiplier;
        public float attackSpeedMultiplier;
        public float damageMultiplier;
        public float armorMultiplier;
        public float armorBonus;
    }

    // for replacing materials
    public class MonsterMaterialReplacement : ScriptableObject
    {
        // which rendererinfo you're trying to replace the material of
        public int rendererIndex;
        // replacement material
        public Material material;
    }

    // for replacing monster meshes
    public class MonsterMeshReplacement : ScriptableObject
    {
        // which rendererinfo you're trying to replace the mesh of
        public int rendererIndex;
        // mesh to replace it with
        public Mesh mesh;
    }

    // for skill replacements
    public class MonsterSkillReplacement : ScriptableObject
    {
        // which skill slot to replace
        public SkillSlot skillSlot;
        // replacement skillDef
        public SkillDef skillDef;
    }

    // for custom inventories
    public class ItemInfo : ScriptableObject
    {
        public string itemString;
        public int count;
    }

    // for size adjustments
    public class MonsterSizeModifier : ScriptableObject
    {
        // the altered size of the monster
        public float newSize;
        // scale colliders as well? useful for flying enemies, gets weird on grounded ones
        public bool scaleCollider;
    }

    public enum MonsterVariantTier
    {
        Common,
        Uncommon,
        Rare
    };

    public enum MonsterVariantAIModifier
    {
        Default,
        Unstable,
        ForceSprint
    };
}