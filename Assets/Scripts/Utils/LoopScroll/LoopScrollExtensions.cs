using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara
{
    public static class LoopScrollExtensions
    {
        private static readonly FieldInfo itemTypeStartField = typeof(LoopScrollRectBase)
            .GetField("itemTypeStart", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool TryAt(this LoopScrollRectBase loopScroll, int index, out Transform transform)
        {
            int itemTypeStart = (int)itemTypeStartField.GetValue(loopScroll);

            int contentIdx = index - itemTypeStart;
            if (contentIdx < 0 || contentIdx >= loopScroll.content.childCount)
            {
                transform = null;
                return false;
            }

            transform = loopScroll.content.GetChild(contentIdx);
            return true;
        }
    }
}