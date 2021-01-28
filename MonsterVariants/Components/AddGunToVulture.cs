using System;
using RoR2;
using UnityEngine;

namespace MonsterVariants.Components
{
    public class AddGunToVulture : MonoBehaviour
    {
        private CharacterModel model;
        private ChildLocator childLocator;

        private void Start()
        {
            this.model = base.GetComponentInChildren<CharacterModel>();
            this.childLocator = base.GetComponentInChildren<ChildLocator>();

            this.AddMissileLauncher();
        }

        private void AddMissileLauncher()
        {
            if (this.model)
            {
                GameObject missileLauncher = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.pistolPrefab, this.childLocator.FindChild("Head"));
                missileLauncher.transform.localPosition = new Vector3(0, 3.5f, 0.5f);
                missileLauncher.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 180));
                missileLauncher.transform.localScale = Vector3.one * 16f;
            }
        }
    }
}