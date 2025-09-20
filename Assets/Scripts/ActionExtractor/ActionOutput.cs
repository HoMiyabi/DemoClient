using System.Collections.Generic;
using Kirara.TimelineAction;

namespace Kirara.ActionExtractor
{
    public class ActionOutput
    {
        public string name;
        public AnimRootMotion rootMotion;
        public bool isLoop;
        public List<BoxNotifyState> boxes = new();
    }
}