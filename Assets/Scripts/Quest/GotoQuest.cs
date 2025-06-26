using System;
using cfg.main;
using Kirara.Indicator;
using Quest;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kirara.Quest
{
    public class GotoQuest : Quest
    {
        public readonly GotoQuestConfig config;
        protected override QuestConfig BaseQuestConfig => config;

        private GameObject go;

        public GotoQuest(GotoQuestConfig config, QuestChain questChain): base(questChain)
        {
            this.config = config;
        }

        public override void Enable()
        {
            base.Enable();
            go = new GameObject("GotoQuest " + config.Name);
            go.transform.position = config.Center.Unity();

            var handler = go.AddComponent<GotoQuestTriggerHandler>();
            handler.Set(this);

            var col = go.AddComponent<SphereCollider>();
            col.includeLayers = LayerConfig.CharacterMask;
            col.isTrigger = true;
            col.radius = config.Radius;
        }

        public override void Disable()
        {
            base.Disable();
            Object.Destroy(go);
        }

        public void OnTriggerEnter()
        {
            questChain.CompleteQuest(this);
        }

        public override void OnTrack()
        {
            base.OnTrack();
            IndicatorSystem.AddIndicator(PlayerSystem.Instance.Player.transform, config.Center.Unity());
        }
    }
}