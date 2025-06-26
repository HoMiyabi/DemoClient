using Kirara.Service;
using UnityEngine;

namespace Kirara
{
    public class DiscInteractable : NormalInteractable
    {
        private void Start()
        {
            headInfo.Name = "捡点驱动盘";
        }

        public override void Interact(Transform interactor)
        {
            DiscService.GachaDisc();
        }
    }
}