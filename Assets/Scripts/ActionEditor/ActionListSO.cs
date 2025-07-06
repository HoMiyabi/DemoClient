using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    // 动作列表，一个角色一般有许多动作，我们希望能在一个地方编辑
    [CreateAssetMenu(fileName = "ActionListSO", menuName = "Kirara/ActionListSO")]
    public class ActionListSO : ScriptableObject
    {
        public List<ActionSO> actions = new();

        public ActionSO AddAction()
        {
            Undo.RecordObject(this, "添加动作");

            var action = CreateInstance<ActionSO>();
            actions.Add(action);
            EditorUtility.SetDirty(this);
            return action;
        }

        public void RemoveActionAt(int index)
        {
            Undo.RecordObject(this, "删除动作");

            actions.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }
    }
}