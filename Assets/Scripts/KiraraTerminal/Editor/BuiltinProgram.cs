using System.Reflection;
using Kirara.TimelineAction;
using UnityEditor;
using UnityEngine;

namespace KiraraTerminal
{
    public static class BuiltinProgram
    {
        [Program("ls", "列出所有程序")]
        public static void List()
        {
            foreach (var program in Shell.programs.Values)
            {
                Debug.Log($"名称：{program.name} 描述：{program.description}");
            }
        }

        [Program("replace", "替换名字")]
        public static void Replace(string oldValue, string newValue)
        {
            var assets = Selection.GetFiltered<Object>(
                SelectionMode.Assets | SelectionMode.Editable);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.RenameAsset(path, asset.name.Replace(oldValue, newValue));
            }
        }

        [Program("add_prefix", "名字添加前缀")]
        public static void AddPrefix(string prefix)
        {
            var assets = Selection.GetFiltered<Object>(
                SelectionMode.Assets | SelectionMode.Editable);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.RenameAsset(path, prefix + asset.name);
            }
        }

        [Program("add_suffix", "名字添加后缀")]
        public static void AddSuffix(string suffix)
        {
            var assets = Selection.GetFiltered<Object>(
                SelectionMode.Assets | SelectionMode.Editable);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.RenameAsset(path, asset.name + suffix);
            }
        }

        [Program("create_action", "创建同名Action")]
        public static void CreateAction()
        {
            var assets = Selection.GetFiltered<Object>(
                SelectionMode.Assets | SelectionMode.Editable);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                var action = ScriptableObject.CreateInstance<KiraraActionSO>();
                action.name = asset.name;
                AssetDatabase.CreateAsset(action, path + ".asset");
            }
        }

        [Program("format_anim_clip")]
        public static void FormatAnimationClip()
        {
            var assets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer.clipAnimations.Length == 1)
                {
                    var clip = importer.clipAnimations[0];
                    clip.name = asset.name.Substring(asset.name.IndexOf("Ani_") + 4);
                    Debug.Log("change to " + clip.name);
                    importer.clipAnimations = new[] { clip };
                    importer.SaveAndReimport();
                }
            }
            AssetDatabase.Refresh();
        }

        [Program("fbx_anim_prefix")]
        public static void FbxAnimPrefix()
        {
            var assets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer.clipAnimations.Length == 1)
                {
                    var clip = importer.clipAnimations[0];
                    clip.name = asset.name.Replace("Longinus_", "");
                    Debug.Log("change to " + clip.name);
                    importer.clipAnimations = new[] { clip };
                    importer.SaveAndReimport();
                }
            }
            AssetDatabase.Refresh();
        }

        [Program("rootmotion")]
        public static void RootMotion()
        {
            var asset = Selection.activeObject;
            var clip = (AnimationClip)asset;

        }
    }
}