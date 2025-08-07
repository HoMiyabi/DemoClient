using UnityEngine;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [CreateAssetMenu(fileName = "TimelineActionSO", menuName = "Kirara/TimelineActionSO")]
    public class  KiraraActionSO : TimelineAsset
    {
        public int actionId;
        public EActionState actionState;
        public ActionParams actionParams;

        public bool isLoop;
        public FinishCancelInfo finishCancelInfo;
    }
}