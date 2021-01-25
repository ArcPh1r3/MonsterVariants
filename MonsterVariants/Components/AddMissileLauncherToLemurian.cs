using System;
using RoR2;
using UnityEngine;

namespace MonsterVariants.Components
{
    public class AddMissileLauncherToLemurian : MonoBehaviour
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
                GameObject missileLauncher = UnityEngine.Object.Instantiate<GameObject>(MainPlugin.missileLauncherDisplayPrefab, this.childLocator.FindChild("Chest"));
                missileLauncher.transform.localPosition = new Vector3(0, 0, 1.75f);
                missileLauncher.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0, 0));
                missileLauncher.transform.localScale = Vector3.one * 8f;
            }
        }
    }
}
