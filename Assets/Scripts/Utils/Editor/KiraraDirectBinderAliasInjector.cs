using System.Collections.Generic;
using KiraraDirectBinder.Editor;
using UnityEditor;
using UnityEngine;

namespace Kirara
{
    public static class KiraraDirectBinderAliasInjector
    {
        [InitializeOnLoadMethod]
        public static void Inject()
        {
            KiraraDirectBinderAlias.alias.Add(typeof(RectTransform), new List<string>{ "Tra" });
            KiraraDirectBinderAlias.alias.Add(typeof(UnityEngine.UI.Button), new List<string>{ "Btn" });
            KiraraDirectBinderAlias.alias.Add(typeof(UnityEngine.UI.Image), new List<string>{ "Img", "Icon" });
            KiraraDirectBinderAlias.alias.Add(typeof(TMPro.TMP_InputField), new List<string>{ "Input" });
            KiraraDirectBinderAlias.alias.Add(typeof(TMPro.TextMeshProUGUI), new List<string>{ "Text" });
            KiraraDirectBinderAlias.alias.Add(typeof(TMPro.TMP_Dropdown), new List<string>{ "Dd" });
            KiraraDirectBinderAlias.alias.Add(typeof(UnityEngine.UI.LoopVerticalScrollRect), new List<string>{ "LoopScroll" });
            KiraraDirectBinderAlias.alias.Add(typeof(UnityEngine.UI.LoopHorizontalScrollRect), new List<string>{ "LoopScroll" });
            KiraraDirectBinderAlias.alias.Add(typeof(UnityEngine.UI.Slider), new List<string> {"Slider"});
        }
    }
}