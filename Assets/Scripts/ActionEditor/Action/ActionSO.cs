using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    // 动作
    public class ActionSO : ScriptableObject
    {
        public string animName;
        public List<ActionTrackSO> tracks = new();

        private string GetTrackName()
        {
            return (tracks.Count + 1).ToString();
        }

        public ActionTrackSO AddTrack()
        {
            var track = CreateInstance<ActionTrackSO>();
            track.name = GetTrackName();
            Undo.RecordObject(this, "添加轨道");
            AssetDatabase.AddObjectToAsset(track, this);
            tracks.Add(track);
            EditorUtility.SetDirty(this);
            return track;
        }

        public void RemoveTrackAt(int index)
        {
            Undo.RecordObject(this, "删除轨道");
            AssetDatabase.RemoveObjectFromAsset(tracks[index]);
            tracks.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }

        public bool RemoveTrack(ActionTrackSO track)
        {
            int idx = tracks.IndexOf(track);
            if (idx >= 0)
            {
                RemoveTrackAt(idx);
                return true;
            }
            return false;
        }
    }
}