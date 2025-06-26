using Kirara.UI;
using UnityEngine;

namespace Kirara
{
    public abstract class NormalInteractable : MonoBehaviour, IInteractable
    {
        private bool select;
        public bool IsSelected
        {
            get => select;
            set
            {
                if (select != value)
                {
                    select = value;
                    headInfo.Selected = value;
                }
            }
        }

        public string interactName;
        public UIHeadInfo headInfo;
        public Vector3 headInfoLocalPos;

        public void SetName(string _name)
        {
            interactName = _name;
            headInfo.Name = _name;
        }

        protected virtual void Awake()
        {
            headInfo = UIMgr.Instance.AddHUD<UIHeadInfo>()
                .Set(interactName, transform, headInfoLocalPos);
        }

        public abstract void Interact(Transform interactor);
    }
}