using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Kirara.TimelineAction
{
    [DisplayName("盒子通知状态")]
    public class BoxNotifyState : ActionNotifyState
    {
        [NonSerialized] public GameObject owner;

        public EBoxType boxType = EBoxType.HitBox;
        public EBoxShape boxShape = EBoxShape.Sphere;
        public Vector3 center = new(0, 1, 1.5f);
        public float radius = 1f;
        public Vector3 size = new(2, 2, 2);
        public EHitStrength hitStrength;
        public int hitId;
        public GameObject particlePrefab;
        [FormerlySerializedAs("setRot")] public bool setParticleRot;
        public float rotValue;
        public float rotMaxValue;
        public float hitGatherDist;
        public AudioClip hitAudio;

        // 命中停顿
        [TimeField(60)]
        public float hitstopDuration = 0.05f;
        public float hitstopSpeed = 0f;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.owner = owner;
            var playable = ScriptPlayable<BoxPlayable>.Create(graph);
            playable.GetBehaviour().asset = this;
            return playable;
        }

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
            {
                monsterCtrl.BoxBegin(this);
            }
            else if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                roleCtrl.BoxBegin(this);
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
            {
                monsterCtrl.BoxEnd(this);
            }

            // HitstopNotify不知道怎么不见了在Timeline里面
        }
    }
}