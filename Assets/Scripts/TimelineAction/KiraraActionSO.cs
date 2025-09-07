using System.Collections.Generic;
using TimelineAction;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CreateAssetMenu(fileName = "TimelineActionSO", menuName = "Kirara/TimelineActionSO")]
    public class KiraraActionSO : TimelineAsset
    {
        public int actionId;
        public EActionState actionState;
        public ActionParams actionParams;

        public bool isLoop;
        [FormerlySerializedAs("finishCancelInfo")] public FinishTransitionInfo finishTransition;

        public string inheritTransitionActionName;
        public List<CommandTransitionInfo> commandTransitions;
        public List<SignalTransitionInfo> signalTransitions;
    }
}