using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    public abstract class ActionNotify : Marker
    {
        public virtual void Notify(ActionCtrl actionCtrl)
        {
        }
    }
}