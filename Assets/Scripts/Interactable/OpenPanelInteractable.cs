using UnityEngine;

namespace Kirara
{
    public class OpenPanelInteractable : NormalInteractable
    {
        public string panelName;

        public override void Interact(Transform interactor)
        {
            UIMgr.Instance.PushPanel(panelName);
        }
    }
}