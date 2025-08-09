using System;
using UnityEngine;

namespace Kirara.UI
{
    public class UIInventoryDetailWrapper : MonoBehaviour
    {
        #region View
        private RectTransform Viewport;
        private void InitUI()
        {
            var c    = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Viewport = c.Q<RectTransform>(0, "Viewport");
        }
        #endregion

        public RectTransform DetailView => Viewport.GetChild(0) as RectTransform;
    }
}