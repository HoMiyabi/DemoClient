using Cysharp.Threading.Tasks;
using Kirara.Service;
using Kirara.System;
using UnityEngine;

namespace Kirara
{
    public class GatherMaterialNPC : NPCInteractable
    {
        public override void Interact(Transform interactor)
        {
            int cid = 1;
            int count = 1;
            InventoryService.GatherMaterial(cid, count);
        }
    }
}