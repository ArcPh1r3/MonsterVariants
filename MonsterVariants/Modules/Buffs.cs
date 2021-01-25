using R2API;
using RoR2;
using UnityEngine;

namespace MonsterVariants.Modules
{
    internal static class Buffs
    {
        internal static BuffIndex toxicBeetleBuff;

        internal static void RegisterBuffs()
        {
            toxicBeetleBuff = AddNewBuff("Toxic", "Textures/BuffIcons/texBuffGenericShield", Color.green, false, false);
        }

        internal static BuffIndex AddNewBuff(string buffName, string iconPath, Color buffColor, bool canStack, bool isDebuff)
        {
            CustomBuff tempBuff = new CustomBuff(new BuffDef
            {
                name = buffName,
                iconPath = iconPath,
                buffColor = buffColor,
                canStack = canStack,
                isDebuff = isDebuff,
                eliteIndex = EliteIndex.None
            });

            return BuffAPI.Add(tempBuff);
        }
    }
}