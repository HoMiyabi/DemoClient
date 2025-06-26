using System.Collections.Generic;
using UnityEngine;

namespace Kirara.Indicator
{
    /// <summary>
    /// 指示器系统
    /// </summary>
    public static class IndicatorSystem
    {
        private static readonly List<UITargetIndicator> indicators = new();

        public static void ClearIndicators()
        {
            foreach (var indicator in indicators)
            {
                Object.Destroy(indicator.gameObject);
            }
            indicators.Clear();
        }

        public static bool RemoveIndicator(UITargetIndicator indicator)
        {
            if (!indicators.Remove(indicator)) return false;

            Object.Destroy(indicator.gameObject);
            return true;
        }

        public static void AddIndicator(Transform cur, Transform target,
            Vector3 showOffset = default, bool hideInCircle = false)
        {
            indicators.Add(UIMgr.Instance.AddHUD<UITargetIndicator>().Set(cur, target, showOffset, hideInCircle));
        }

        public static void AddIndicator(Transform cur, Vector3 target,
            Vector3 showOffset = default, bool hideInCircle = false)
        {
            indicators.Add(UIMgr.Instance.AddHUD<UITargetIndicator>().Set(cur, target, showOffset, hideInCircle));
        }
    }
}