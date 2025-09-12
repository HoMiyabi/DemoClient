using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    [DisplayName("粒子控制")]
    public class ParticleControlNotifyState : ActionNotifyState
    {
        [NonSerialized] public GameObject owner;
        public GameObject prefab;
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.one;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.owner = owner;
            return ParticleControlPlayable.Create(graph, owner, prefab, position, rotation, scale);
        }

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            var transform = actionCtrl.transform;
            ParticleMgr.Instance.PlayAsChild(prefab, transform, position, rotation, scale);
        }
    }
}