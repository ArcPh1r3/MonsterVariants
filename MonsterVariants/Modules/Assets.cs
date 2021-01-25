using System.Reflection;
using R2API;
using UnityEngine;

namespace MonsterVariants.Modules
{
    internal static class Assets
    {
        internal static AssetBundle mainAssetBundle = null;

        internal static Mesh armoredMesh;
        internal static Mesh speedyBeetleMesh;

        internal static void PopulateAssets()
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