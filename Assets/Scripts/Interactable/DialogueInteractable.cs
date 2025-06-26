using Kirara.UI.Panel;
using UnityEngine;

namespace Kirara
{
    public class DialogueInteractable : NormalInteractable
    {
        public int dialogueId;

        public override void Interact(Transform interactor)
        {
            UIMgr.Instance.PushPanel<DialoguePanel>().Set(dialogueId, interactor, transform);
        }
    }
}