using System.Reflection;
using R2API;
using UnityEngine;

namespace MonsterVariants.Modules
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static Mesh armoredMesh;
        public static Mesh speedyBeetleMesh;

        public static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonsterVariants.monstervariants"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    var provider = new AssetBundleResourcesProvider("@MonsterVariants", mainAssetBundle);
                    ResourcesAPI.AddProvider(provider);
                }
            }

            armoredMesh = mainAssetBundle.LoadAsset<Mesh>("meshArmoredBeetle");
            speedyBeetleMesh = mainAssetBundle.LoadAsset<Mesh>("meshSpeedyBeetle");
        }
    }
}