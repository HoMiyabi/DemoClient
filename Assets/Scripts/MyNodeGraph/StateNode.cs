using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using XNode;

namespace Kirara
{
    [NodeWidth(500)]
    public class StateNode : Node
    {
        [Input]
        public bool enter;
        public TimelineAsset action;
        public float time;

        [Output(dynamicPortList = true)]
        public List<NodeTransition> transitions;

        public override object GetValue(NodePort port)
        {
            return true;
        }
    }
}