using Kirara.Service;
using UnityEngine;

namespace Kirara
{
    public class WeaponInteractable : NormalInteractable
    {
        private void Start()
        {
            headInfo.Name = "捡点武器";
        }

        public override void Interact(Transform interactor)
        {
            WeaponService.GachaWeapon();
        }
    }
}