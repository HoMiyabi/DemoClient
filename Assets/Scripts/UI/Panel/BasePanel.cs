using System;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class BasePanel : MonoBehaviour
    {
        public Action onClosed;

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

        public virtual void OnPause()
        {
        }

        public virtual void OnResume()
        {
        }

        public void PopPanel()
        {
            UIMgr.Instance.PopPanel(this);
        }
    }
}