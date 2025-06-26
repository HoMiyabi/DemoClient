using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kirara.UI.Editor
{
    public class KiraraRuntimeComponentsBinderWindow : EditorWindow
    {
        private static readonly Dictionary<Type, List<string>> alias = new()
        {
            {typeof(RectTransform), new List<string>{ "Tra" }},
            {typeof(UnityEngine.UI.Button), new List<string>{ "Btn" }},
            {typeof(UnityEngine.UI.Image), new List<string>{ "Img", "Icon" }},
            {typeof(TMPro.TMP_InputField), new List<string>{ "Input" }},
            {typeof(TMPro.TextMeshProUGUI), new List<string>{ "Text" }},
            {typeof(TMPro.TMP_Dropdown), new List<string>{ "Dd" }},
            {typeof(UnityEngine.UI.LoopVerticalScrollRect), new List<string>{ "LoopScroll" }},
            {typeof(UnityEngine.UI.LoopHorizontalScrollRect), new List<string>{ "LoopScroll" }},
            {typeof(UnityEngine.UI.Slider), new List<string>{ "Slider" }}
        };

        private KiraraRuntimeComponents rtCom;
        private KiraraRuntimeComponents RtCom
        {
            get => rtCom;
            set
            {
                if (value == rtCom) return;

                rtCom = value;
                Set();
            }
        }
        private Transform target;
        private Transform Target
        {
            get => target;
            set
            {
                if (value == target) return;

                target = value;
                Set();
            }
        }

        private Component[] components;
        private string[] fieldNames;
        private int[] comIdx;


        [MenuItem("GameObject/KiraraRuntimeComponentsBinderWindow _c", false, priority = 0)]
        public static void OpenWindow()
        {
            var tra = Selection.activeTransform;

            if (tra == null) return;

            var window = GetWindow<KiraraRuntimeComponentsBinderWindow>(
                true,
                nameof(KiraraRuntimeComponentsBinderWindow));
            window.target = tra;
            window.rtCom = FindRtCom(tra);
            window.Set();
        }

        private void Set()
        {
            components = GetComponents(target);
            SetComInfo();

            SetWindowPos();
        }

        private void SetComInfo()
        {
            if (components == null || rtCom == null) return;

            int n = components.Length;

            comIdx = new int[n];
            fieldNames = new string[n];

            for (int i = 0; i < n; i++)
            {
                comIdx[i] = rtCom.items
                    .FindIndex(x => x.component == components[i]);
                if (comIdx[i] != -1)
                {
                    fieldNames[i] = rtCom.items[comIdx[i]].fieldName;
                }
                else
                {
                    fieldNames[i] = components[i].name.Replace(" ", "").Replace("(", "").Replace(")", "");
                }
            }
        }

        private void SetWindowPos()
        {
            var windowRect = position;

            windowRect.position = Event.current.mousePosition + new Vector2(-15, -10);

            position = windowRect;
        }

        private static int MatchScore(Component com, string target)
        {
            var type = com.GetType();
            string name = type.Name;
            if (target.EndsWith(name))
            {
                return 3;
            }

            string pattern = $@"{Regex.Escape(name)} \(\d+\)$";
            if (Regex.IsMatch(target, pattern))
            {
                return 2;
            }

            if (alias.TryGetValue(type, out var aliasNames)
                && aliasNames.Any(target.EndsWith))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 把匹配的组件放在前面
        /// </summary>
        /// <param name="tra"></param>
        /// <returns></returns>
        private static Component[] GetComponents(Transform tra)
        {
            if (tra == null) return null;

            string _name = tra.name;
            var coms = tra.GetComponents<Component>();
            Array.Sort(coms, (x, y) =>
                MatchScore(y, _name) - MatchScore(x, _name));
            return coms;
        }

        /// <summary>
        /// 优先在父路径找最近的，其次自己上查找 <see cref="KiraraRuntimeComponents"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        private static KiraraRuntimeComponents FindRtCom(Transform transform)
        {
            if (transform == null) return null;
            var p = transform.parent;
            KiraraRuntimeComponents rtComs;

            while (p != null)
            {
                if (p.TryGetComponent<KiraraRuntimeComponents>(out rtComs))
                {
                    return rtComs;
                }
                p = p.parent;
            }
            if (transform.TryGetComponent<KiraraRuntimeComponents>(out rtComs))
            {
                return rtComs;
            }
            return null;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rt Com: ", GUILayout.ExpandWidth(false));
            RtCom = (KiraraRuntimeComponents)EditorGUILayout.ObjectField(
                RtCom, typeof(KiraraRuntimeComponents), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"选中对象: ", GUILayout.ExpandWidth(false));
            Target = (Transform)EditorGUILayout.ObjectField(
                Target, typeof(Transform), true);
            GUILayout.EndHorizontal();


            float maxTypeWidth = components
                .Select(x =>
                    GUI.skin.label.CalcSize(new GUIContent(x.GetType().Name)).x)
                .Max();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                Close();
            }
            GUILayout.Label("字段名");
            GUILayout.Label("类型", GUILayout.Width(maxTypeWidth));
            GUILayout.EndHorizontal();

            for (int i = 0; i < components.Length; i++)
            {
                var com = components[i];

                int idx = comIdx[i];

                GUILayout.BeginHorizontal();

                if (idx == -1)
                {
                    if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                    {
                        Undo.RecordObject(rtCom, $"添加{nameof(rtCom)}" +
                                                       $".{nameof(rtCom.items)}" +
                                                       $"字段{fieldNames[i]} " +
                                                       $"类型{com.GetType().Name}");
                        rtCom.items.Add(new KiraraRuntimeComponents.Item(fieldNames[i], com));
                        EditorSceneManager.MarkSceneDirty(rtCom.gameObject.scene);
                        Close();
                    }

                    fieldNames[i] = GUILayout.TextField(fieldNames[i]);
                }
                else
                {
                    if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                    {
                        Undo.RecordObject(rtCom, $"删除{nameof(rtCom)}" +
                                                       $".{nameof(rtCom.items)}" +
                                                       $"字段{rtCom.items[idx].fieldName} " +
                                                       $"类型{com.GetType().Name}");
                        rtCom.items.RemoveAt(idx);
                        EditorSceneManager.MarkSceneDirty(rtCom.gameObject.scene);
                        Close();
                    }

                    fieldNames[i] = GUILayout.TextField(fieldNames[i]);

                    if (GUILayout.Button("✓", GUILayout.ExpandWidth(false)))
                    {
                        Undo.RecordObject(rtCom, $"修改{nameof(rtCom)}" +
                                                       $".{nameof(rtCom.items)}" +
                                                       $"字段新名字{fieldNames[i]} " +
                                                       $"类型{com.GetType().Name}");
                        rtCom.items[idx].fieldName = fieldNames[i];
                        EditorSceneManager.MarkSceneDirty(rtCom.gameObject.scene);
                    }
                }

                GUILayout.Label(com.GetType().Name, GUILayout.Width(maxTypeWidth));

                GUILayout.EndHorizontal();
            }
        }
    }
}