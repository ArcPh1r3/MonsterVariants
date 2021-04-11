using System.Reflection;
using UnityEngine;

namespace MonsterVariants.Modules
{
    internal static class Assets
    {
        internal static AssetBundle mainAssetBundle = null;

        internal static Mesh armoredMesh;
        internal static Mesh speedyBeetleMesh;

        internal static GameObject pistolPrefab;

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonsterVariants.monstervariants"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            armoredMesh = mainAssetBundle.LoadAsset<Mesh>("meshArmoredBeetle");
            speedyBeetleMesh = mainAssetBundle.LoadAsset<Mesh>("meshSpeedyBeetle");

            pistolPrefab = mainAssetBundle.LoadAsset<GameObject>("VulturePistol");
            pistolPrefab.GetComponentInChildren<MeshRenderer>().material.shader = Resources.Load<Shader>("Shaders/Deferred/hgstandard");
        }
    }
}