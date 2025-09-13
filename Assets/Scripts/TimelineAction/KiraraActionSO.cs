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

        [FormerlySerializedAs("actionParams")]
        public ActionArgs actionArgs;

        [FormerlySerializedAs("finishCancelInfo")]
        public FinishTransitionInfo finishTransition;

        [FormerlySerializedAs("inheritTransitionActionName")]
        [TimelineActionName]
        public string inheritActionTransition;
        public List<CommandTransitionInfo> commandTransitions;
        public List<SignalTransitionInfo> signalTransitions;
    }
}