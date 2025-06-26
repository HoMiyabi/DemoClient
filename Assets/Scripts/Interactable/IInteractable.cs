using UnityEngine;

namespace Kirara
{
    public interface IInteractable
    {
        bool IsSelected { get; set; }
        void Interact(Transform interactor);
    }
}