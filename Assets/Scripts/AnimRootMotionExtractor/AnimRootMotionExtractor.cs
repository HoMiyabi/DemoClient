﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Kirara
{
    public class AnimRootMotionExtractor : EditorWindow
    {
        [MenuItem("Kirara/动画RootMotion提取器")]
        private static void GetWindow()
        {
            GetWindow<AnimRootMotionExtractor>();
        }

        private static List<float> GetKeys(AnimRootMotion motion, string bindingPropertyName)
        {
            return bindingPropertyName switch
            {
                "RootT.x" => motion.tx,
                "RootT.y" => motion.ty,
                "RootT.z" => motion.tz,
                "RootQ.x" => motion.qx,
                "RootQ.y" => motion.qy,
                "RootQ.z" => motion.qz,
                "RootQ.w" => motion.qw,
                _ => throw new ArgumentException("Invalid binding property name", bindingPropertyName)
            };
        }

        private static AnimRootMotion ExtractRootMotion(AnimationClip clip)
        {
            var motion = new AnimRootMotion();
            motion.length = clip.length;
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.path == "" && binding.propertyName.StartsWith("Root"))
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    var keys = GetKeys(motion, binding.propertyName);
                    for (float time = 0f; time < clip.length; time += 1f / 60f)
                    {
                        keys.Add(curve.Evaluate(time));
                    }
                }
            }
            return motion;
        }

        public void OnGUI()
        {
            var clips = Selection.GetFiltered<AnimationClip>(SelectionMode.Unfiltered);
            GUILayout.Label($"已选择{clips.Length}个动画片段");

            if (GUILayout.Button("提取"))
            {
                foreach (var clip in clips)
                {
                    var motion = ExtractRootMotion(clip);
                    string json = JsonConvert.SerializeObject(motion, Formatting.Indented);
                    string path = AssetDatabase.GetAssetPath(clip);
                    path = Path.ChangeExtension(path, "json");
                    Debug.Log($"{clip.name}输出: {path}");
                    File.WriteAllText(path, json);
                }
                AssetDatabase.Refresh();
            }

            // clip = EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false) as AnimationClip;
            // EditorGUILayout.LabelField("Curves:");
            // if (clip != null)
            // {
            //     scrollPos = GUILayout.BeginScrollView(scrollPos);
            //     foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            //     {
            //         AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            //         EditorGUILayout.LabelField($"Path: {binding.path}.{binding.propertyName}, curve.length: {curve.length}, Type: {binding.type}");
            //         if (binding.path == "" && binding.propertyName.StartsWith("Root"))
            //         {
            //             EditorGUILayout.LabelField(string.Join("|", curve.keys.Select(x => $"time:{x.time}, value:{x.value}")));
            //         }
            //     }
            //     GUILayout.EndScrollView();
            // }
        }
    }
}