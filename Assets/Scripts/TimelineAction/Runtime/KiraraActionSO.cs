using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CreateAssetMenu(fileName = "TimelineActionSO", menuName = "Kirara/TimelineActionSO")]
    public class KiraraActionSO : TimelineAsset
    {
        // 给动作Id，判断动作是否可执行用
        public int actionId;

        public string actionType;

        public bool isLoop;

        [FormerlySerializedAs("actionParams")]
        public ActionArgs actionArgs;

        [FormerlySerializedAs("finishCancelInfo")]
        public TransitionInfo finishTransition;

        [FormerlySerializedAs("inheritTransitionActionName")]
        [TimelineActionName]
        public string inheritActionTransition;
        public List<CommandTransitionInfo> commandTransitions;
        public List<SignalTransitionInfo> signalTransitions;
    }
}