using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    [DisplayName("盒子")]
    public class BoxPlayableAsset : ActionNotifyState
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
        public bool setRot;
        public float rotValue;
        public float rotMaxValue;
        public float hitGatherDist;
        public AudioClip hitAudio;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<BoxPlayable>.Create(graph);
            playable.GetBehaviour().asset = this;
            return playable;
        }

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
            {
                monsterCtrl.BoxBegin(this);
                return;
            }

            if (actionCtrl.TryGetComponent<RoleCtrl>(out var roleCtrl))
            {
                roleCtrl.BoxBegin(this);
                return;
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            if (actionCtrl.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
            {
                monsterCtrl.BoxEnd(this);
                return;
            }

            // HitstopNotify不知道怎么不见了在Timeline里面
        }
    }
}