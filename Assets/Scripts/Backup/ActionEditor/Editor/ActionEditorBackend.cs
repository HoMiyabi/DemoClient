/*using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionEditorBackend
    {
        private static ActionEditorBackend instance;
        public static ActionEditorBackend Instance
        {
            get
            {
                instance ??= new ActionEditorBackend();
                return instance;
            }
        }

        public GameObject Prefab { get; set; }

        private ActionListSO actionList;
        public ActionListSO ActionList
        {
            get => actionList;
            set
            {
                if (actionList == value) return;

                actionList = value;
                Action = value != null ? SafeFirstOrDefault(value.actions) : null;
            }
        }

        private ActionSO _action;
        public ActionSO Action
        {
            get => _action;
            set
            {
                if (_action == value) return;

                _action = value;
                OnActionChanged?.Invoke();
                Track = value != null ? SafeFirstOrDefault(value.tracks) : null;
            }
        }
        public event Action OnActionChanged;

        private ActionTrackSO _track;
        public ActionTrackSO Track
        {
            get => _track;
            set
            {
                if (_track == value) return;

                _track = value;
                OnTrackChanged?.Invoke();
            }
        }
        public event Action OnTrackChanged;

        public ActionTrackSO AddTrack(Type type)
        {
            var track = Action.AddTrack(type);
            Track = track;
            return track;
        }

        private static T SafeFirstOrDefault<T>(List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }
    }
}*/