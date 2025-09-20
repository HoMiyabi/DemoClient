using System;
using System.Collections.Generic;
using System.IO;
using Kirara.ActionExtractor;
using Kirara.TimelineAction;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Kirara
{
    public class ActionExtractorWindow : EditorWindow
    {
        private DefaultAsset outputDirAsset;

        [MenuItem("Kirara/动作提取器")]
        private static void GetWindow()
        {
            GetWindow<ActionExtractorWindow>();
        }

        private static int frameRate = 60;

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
            motion.frameRate = frameRate;
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.path == "" && binding.propertyName.StartsWith("Root"))
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    var keys = GetKeys(motion, binding.propertyName);
                    for (float time = 0f; time < clip.length; time += 1f / frameRate)
                    {
                        keys.Add(curve.Evaluate(time));
                    }
                }
            }
            return motion;
        }

        private static ActionOutput ExtractAction(KiraraActionSO action)
        {
            AnimationClip clip = null;
            foreach (var track in action.GetOutputTracks())
            {
                if (track is AnimationTrack animationTrack)
                {
                    foreach (var timelineClip in animationTrack.GetClips())
                    {
                        var asset = (AnimationPlayableAsset)timelineClip.asset;
                        clip = asset.clip;
                        break;
                    }
                }
            }
            if (clip == null)
            {
                Debug.LogWarning($"{action.name}没有动画片段");
            }
            var output = new ActionOutput
            {
                name = action.name,
                rootMotion = ExtractRootMotion(clip),
                isLoop = action.isLoop
            };
            foreach (var track in action.GetOutputTracks())
            {
                if (track is BoxTrack boxTrack)
                {
                    foreach (var timelineClip in boxTrack.GetClips())
                    {
                        var asset = (BoxNotifyState)timelineClip.asset;
                        asset.start = (float)timelineClip.start;
                        asset.length = (float)timelineClip.duration;
                        output.boxes.Add((BoxNotifyState)timelineClip.asset);
                    }
                }
            }
            return output;
        }

        public void OnGUI()
        {
            outputDirAsset = (DefaultAsset)EditorGUILayout.ObjectField("输出目录", outputDirAsset,
                typeof(DefaultAsset), false);
            string outputDir = null;
            if (outputDirAsset != null)
            {
                outputDir = AssetDatabase.GetAssetPath(outputDirAsset);
                if (!Directory.Exists(outputDir))
                {
                    outputDir = null;
                }
            }

            var actions = Selection.GetFiltered<KiraraActionSO>(SelectionMode.Unfiltered);
            GUILayout.Label($"已选择{actions.Length}个动作");

            EditorGUI.BeginDisabledGroup(outputDir == null);
            if (GUILayout.Button("提取"))
            {
                bool overwrite = false;
                foreach (var action in actions)
                {
                    var output = ExtractAction(action);

                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new Vector3JsonConverter());
                    settings.Converters.Add(new BoxNotifyStateJsonConverter());
                    string json = JsonConvert.SerializeObject(output, Formatting.Indented, settings);


                    string outputPath = Path.Combine(outputDir!, action.name + ".json");
                    if (!overwrite && File.Exists(outputPath))
                    {
                        int option = EditorUtility.DisplayDialogComplex(
                            "提示",
                            $"存在同名文件，位于{outputPath}",
                            "覆盖",
                            "跳过",
                            "覆盖且不再提示");
                        switch (option)
                        {
                            case 0:
                                break;
                            case 1:
                                continue;
                            case 2:
                                overwrite = true;
                                break;
                        }
                    }
                    Debug.Log($"{action.name}输出: {outputPath}");
                    File.WriteAllText(outputPath, json);
                }
                AssetDatabase.Refresh();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}