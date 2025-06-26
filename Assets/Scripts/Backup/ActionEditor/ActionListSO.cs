/*using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    // 动作列表，一个角色一般有许多动作，我们希望能在一个地方编辑
    [CreateAssetMenu(fileName = "ActionListSO", menuName = "Kirara/ActionListSO")]
    public class ActionListSO : ScriptableObject
    {
        public List<ActionSO> actions;

        public ActionSO AddAction()
        {
            var action = CreateInstance<ActionSO>();
            action.name = "ActionSO";
            actions ??= new List<ActionSO>();
            actions.Add(action);

            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();

            return action;
        }

        public void RemoveAction(int index)
        {
            var action = actions[index];
            actions.RemoveAt(index);

            AssetDatabase.RemoveObjectFromAsset(action);
            DestroyImmediate(action, true);
            AssetDatabase.SaveAssets();
        }
    }
}*/