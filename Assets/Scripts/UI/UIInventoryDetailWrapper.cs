using System;
using UnityEngine;

namespace Kirara.UI
{
    public class UIInventoryDetailWrapper : MonoBehaviour
    {
        private RectTransform Viewport;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Viewport = c.Q<RectTransform>("Viewport");
        }

        public RectTransform DetailView => Viewport.GetChild(0) as RectTransform;
    }
}