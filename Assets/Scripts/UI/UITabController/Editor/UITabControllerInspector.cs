using System;
using UnityEditor;
using UnityEngine;

namespace Kirara.UI.Editor
{
    [CustomEditor(typeof(UITabController))]
    public class UITabControllerInspector : UnityEditor.Editor
    {
        private UITabController _target;

        private void OnEnable()
        {
            _target = target as UITabController;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tabView = _target.TabView;
            if (tabView == null) return;

            for (int i = 0; i < tabView.childCount; i++)
            {
                var child = tabView.GetChild(i);
                if (GUILayout.Button("激活 " + child.name))
                {
                    for (int j = 0; j < tabView.childCount; j++)
                    {
                        tabView.GetChild(j).gameObject.SetActive(j == i);
                    }
                }
            }
        }
    }
}