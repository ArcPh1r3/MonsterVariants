using BepInEx.Configuration;
using System;

namespace MonsterVariants.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> armoredBeetleSpawnRate;
        public static ConfigEntry<float> speedyBeetleSpawnRate;

        public static ConfigEntry<float> flamethrowerLemurianSpawnRate;

        public static ConfigEntry<float> infernalWispSpawnRate;

        public static ConfigEntry<float> fullAutoGolemSpawnRate;
        public static ConfigEntry<float> titanletSpawnRate;
        public static ConfigEntry<float> overchargedGolemSpawnRate;

        public static ConfigEntry<float> nuclearJellyfishSpawnRate;
        public static ConfigEntry<float> cursedJellyfishSpawnRate;
        public static ConfigEntry<float> spectralJellyfishSpawnRate;

        public static ConfigEntry<float> beetleGuardBruteSpawnRate;

        public static void ReadConfig()
        {
            armoredBeetleSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Armored Beetle"), 10f, new ConfigDescription("Chance for Armored Beetle variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
            speedyBeetleSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Speedy Beetle"), 30f, new ConfigDescription("Chance for Speedy Beetle variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));

            flamethrowerLemurianSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Flamethrower Lemurian"), 2f, new ConfigDescription("Chance for Flamethrower Lemurian variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));

            infernalWispSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Infernal Wisp"), 2f, new ConfigDescription("Chance for Infernal Wisp variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));

            fullAutoGolemSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Full-Auto Golem"), 4f, new ConfigDescription("Chance for Full-Auto Golem variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
            titanletSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Titanlet"), 2f, new ConfigDescription("Chance for Titanlet variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
            overchargedGolemSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Overcharged Golem"), 2f, new ConfigDescription("Chance for Overcharged Golem variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));

            nuclearJellyfishSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Nuclear Jellyfish"), 5f, new ConfigDescription("Chance for Nuclear Jellyfish variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
            cursedJellyfishSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Cursed Jellyfish"), 1f, new ConfigDescription("Chance for Cursed Jellyfish variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
            spectralJellyfishSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Spectral Jellyfish"), 4f, new ConfigDescription("Chance for Spectral Jellyfish variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));

            nuclearJellyfishSpawnRate = MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", "Beetle Guard Brute"), 25f, new ConfigDescription("Chance for Beetle Guard Brute variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
        }
    }
}