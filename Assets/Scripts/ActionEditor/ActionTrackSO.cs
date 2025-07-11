using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionTrackSO : ScriptableObject
    {
        public List<ActionNotifySO> notifies = new();
        public List<ActionNotifyStateSO> states = new();

        public void AddActionNotify()
        {
            var notify = CreateInstance<ActionNotifySO>();
            Undo.RecordObject(this, "添加通知");
            AssetDatabase.AddObjectToAsset(notify, this);
            notifies.Add(notify);
            EditorUtility.SetDirty(this);
        }
    }
}