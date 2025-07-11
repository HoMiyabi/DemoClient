using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    // 动作列表，包含多个动作
    [CreateAssetMenu(fileName = "NewActionList", menuName = "Kirara/NewActionList")]
    public class ActionListSO : ScriptableObject
    {
        public List<ActionSO> actions = new();

        public ActionSO AddAction()
        {
            var action = CreateInstance<ActionSO>();
            Undo.RecordObject(this, "添加动作");
            AssetDatabase.AddObjectToAsset(action, this);
            actions.Add(action);
            EditorUtility.SetDirty(this);
            return action;
        }

        public void RemoveActionAt(int index)
        {
            Undo.RecordObject(this, "删除动作");
            AssetDatabase.RemoveObjectFromAsset(actions[index]);
            actions.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }
    }
}