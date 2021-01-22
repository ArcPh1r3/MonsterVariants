using R2API;
using RoR2;
using UnityEngine;

namespace MonsterVariants.Modules
{
    public static class Buffs
    {
        public static BuffIndex toxicBeetleBuff;

        public static void RegisterBuffs()
        {
            BuffDef toxicBuff = new BuffDef
            {
                name = "Toxic",
                iconPath = "Textures/BuffIcons/texBuffGenericShield",
                buffColor = Color.green,
                canStack = false,
                isDebuff = false,
                eliteIndex = EliteIndex.None
            };

            CustomBuff toxic = new CustomBuff(toxicBuff);
            toxicBeetleBuff = BuffAPI.Add(toxic);
        }
    }
}