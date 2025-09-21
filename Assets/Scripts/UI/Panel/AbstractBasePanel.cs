using System;
using UnityEngine;
using XLua;

namespace Kirara.UI.Panel
{
    [LuaCallCSharp]
    public abstract class AbstractBasePanel : MonoBehaviour
    {
        public Action onPlayEnterFinished;
        public Action onPlayExitFinished;

        public virtual void PlayEnter()
        {
            onPlayEnterFinished?.Invoke();
        }

        public virtual void PlayExit()
        {
            onPlayExitFinished?.Invoke();
        }
        public virtual void OnPause() {}
        public virtual void OnResume() {}
    }
}