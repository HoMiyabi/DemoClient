using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    // 动作
    public class ActionSO : ScriptableObject
    {
        public string animName;
        public List<ActionTrack> tracks = new();

        private string GetTrackName()
        {
            return (tracks.Count + 1).ToString();
        }

        public ActionTrack AddTrack()
        {
            Undo.RecordObject(this, "添加轨道");

            var track = CreateInstance<ActionTrack>();
            tracks.Add(track);
            EditorUtility.SetDirty(this);
            return track;
        }

        public void RemoveTrackAt(int index)
        {
            Undo.RecordObject(this, "删除轨道");

            tracks.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }
    }
}