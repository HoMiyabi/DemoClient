using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    [DisplayName("粒子控制")]
    public class ParticleControlPlayableAsset : ActionNotifyState
    {
        [NonSerialized] public GameObject owner;
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.owner = owner;
            return ParticleControlPlayable.Create(graph, owner, prefab, position, rotation, scale);
        }

        public override void NotifyBegin(ActionPlayer player)
        {
            var transform = player.transform;
            var particle = ParticleMgr.Instance.Play(prefab, transform);
            particle.transform.localPosition = position;
            particle.transform.localRotation = Quaternion.Euler(rotation);
            particle.transform.localScale = scale;
        }
    }
}