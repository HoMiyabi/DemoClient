using Animancer;
using UnityEngine;

namespace Kirara
{
    public class LoginSceneEntry : MonoBehaviour
    {
        private void Awake()
        {
            Animancer.OptionalWarning.DuplicateEvent.Disable();
        }
    }
}