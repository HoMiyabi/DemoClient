using System.Collections.Generic;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionTrack : ScriptableObject
    {
        public List<ActionNotifySO> notifies = new();
        public List<ActionNotifyStateSO> states = new();
    }
}