using System.ComponentModel;
using Kirara.TimelineAction;
using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    [DisplayName("取消窗口")]
    public class CancelWindowPlayableAsset : ActionNotifyState
    {
        public float inputBufferDuration = 0.1f;
        public CancelInfo cancelInfo;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CancelWindowPlayable>.Create(graph);
        }

        public override void NotifyBegin(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl>();
            if (actionCtrl == null)
            {
                Debug.LogWarning("ActionCtrl is null");
                return;
            }
            actionCtrl.cancelWindowsAsset.Add(this);
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl>();
            if (actionCtrl == null)
            {
                Debug.LogWarning("ActionCtrl is null");
                return;
            }
            actionCtrl.cancelWindowsAsset.Remove(this);
        }

        public bool Check(EActionCommand command, EActionCommandPhase phase, float time)
        {
            return cancelInfo.command == command &&
                   cancelInfo.phase == phase &
                   time >= start &&
                   time < start + length;
        }

        public bool Inside(float time)
        {
            return time >= start &&
                   time < start + length;
        }
    }
}