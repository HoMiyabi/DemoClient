using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionTrackSO : ScriptableObject
    {
        public List<ActionNotifySO> notifies = new();
        public List<ActionNotifyStateSO> notifyStates = new();

        public void AddActionNotify(Type type)
        {
            if (!type.IsSubclassOf(typeof(ActionNotifySO)))
            {
                Debug.LogError("Notify type must be subclass of ActionNotifySO");
                return;
            }
            var notify = (ActionNotifySO)CreateInstance(type);
            Undo.RecordObject(this, "添加动作通知");
            AssetDatabase.AddObjectToAsset(notify, this);
            notifies.Add(notify);
            EditorUtility.SetDirty(this);
        }

        public void AddActionNotifyState(Type type)
        {
            if (!type.IsSubclassOf(typeof(ActionNotifyStateSO)))
            {
                Debug.LogError("Notify state type must be subclass of ActionNotifyStateSO");
                return;
            }
            var state = (ActionNotifyStateSO)CreateInstance(type);
            Undo.RecordObject(this, "添加动作通知状态");
            AssetDatabase.AddObjectToAsset(state, this);
            notifyStates.Add(state);
            EditorUtility.SetDirty(this);
        }
    }
}