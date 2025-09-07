using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CreateAssetMenu(fileName = "TimelineActionSO", menuName = "Kirara/TimelineActionSO")]
    public class KiraraActionSO : TimelineAsset
    {
        public int actionId;

        public bool isLoop;
        [FormerlySerializedAs("finishCancelInfo")] public FinishTransitionInfo finishTransition;

        [TimelineActionName]
        public string inheritTransitionActionName;
        public ActionParams actionParams;
        public List<CommandTransitionInfo> commandTransitions;
        public List<SignalTransitionInfo> signalTransitions;
    }
}