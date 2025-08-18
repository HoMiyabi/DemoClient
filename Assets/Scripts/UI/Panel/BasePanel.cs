using System;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class BasePanel : AbstractBasePanel
    {
        public virtual void BindUI()
        {
        }

        protected virtual void Awake()
        {
            BindUI();
        }
    }
}