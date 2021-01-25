using BepInEx.Configuration;
using System;

namespace MonsterVariants.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> armoredBeetleSpawnRate;
        public static ConfigEntry<float> speedyBeetleSpawnRate;

        public static ConfigEntry<float> flamethrowerLemurianSpawnRate;
        public static ConfigEntry<float> badassLemurianSpawnRate;

        public static ConfigEntry<float> infernalWispSpawnRate;

        public static ConfigEntry<float> fullAutoGolemSpawnRate;
        public static ConfigEntry<float> titanletSpawnRate;
        public static ConfigEntry<float> overchargedGolemSpawnRate;

        public static ConfigEntry<float> nuclearJellyfishSpawnRate;
        public static ConfigEntry<float> cursedJellyfishSpawnRate;
        public static ConfigEntry<float> spectralJellyfishSpawnRate;
        public static ConfigEntry<float> MOAJSpawnRate;

        public static ConfigEntry<float> beetleGuardBruteSpawnRate;

        public static ConfigEntry<float> clottedImpSpawnRate;

        public static ConfigEntry<float> artilleryVultureSpawnRate;

        public static ConfigEntry<float> speedyBisonSpawnRate;
        public static ConfigEntry<float> albinoBisonSpawnRate;

        public static ConfigEntry<float> colossalTitanSpawnRate;
        public static ConfigEntry<float> golemletSpawnRate;

        public static void ReadConfig()
        {
            armoredBeetleSpawnRate = SpawnRateConfig("Armored Beetle", 10f);
            speedyBeetleSpawnRate = SpawnRateConfig("Speedy Beetle", 30f);

            flamethrowerLemurianSpawnRate = SpawnRateConfig("Flamethrower Lemurian", 2f);
            badassLemurianSpawnRate = SpawnRateConfig("Badass Lemurian", 2f);

            infernalWispSpawnRate = SpawnRateConfig("Infernal Wisp", 2f);

            fullAutoGolemSpawnRate = SpawnRateConfig("Full-Auto Golem", 4f);
            titanletSpawnRate = SpawnRateConfig("Titanlet", 2f);
            overchargedGolemSpawnRate = SpawnRateConfig("Overcharged Golem", 2f);

            nuclearJellyfishSpawnRate = SpawnRateConfig("Nuclear Jellyfish", 5f);
            cursedJellyfishSpawnRate = SpawnRateConfig("Cursed Jellyfish", 1f);
            spectralJellyfishSpawnRate = SpawnRateConfig("Spectral Jellyfish", 4f);
            MOAJSpawnRate = SpawnRateConfig("M.O.A.J", 2f);

            //oops
            if (nuclearJellyfishSpawnRate.Value == 25f) nuclearJellyfishSpawnRate.Value = 5f;

            beetleGuardBruteSpawnRate = SpawnRateConfig("Beetle Guard Brute", 25f);

            clottedImpSpawnRate = SpawnRateConfig("Clotted Imp", 5f);

            artilleryVultureSpawnRate = SpawnRateConfig("Artillery Vulture", 3f);

            speedyBisonSpawnRate = SpawnRateConfig("Speedy Bison", 30f);
            albinoBisonSpawnRate = SpawnRateConfig("Albino Bison", 4f);

            golemletSpawnRate = SpawnRateConfig("Golemlet", 2f);
            colossalTitanSpawnRate = SpawnRateConfig("Colossal Titan", 2f);
        }

        private static ConfigEntry<float> SpawnRateConfig(string enemyName, float defaultValue)
        {
            return MainPlugin.instance.Config.Bind<float>(new ConfigDefinition("01 - Spawn Rates", enemyName), defaultValue, new ConfigDescription("Chance for " + enemyName + " variant to spawn (percentage, 1-100)", null, Array.Empty<object>()));
        }
    }
}