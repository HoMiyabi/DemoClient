using System;
using cfg.main;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;

namespace Kirara
{
    public class NPCInteractable : NormalInteractable
    {
        public int npcCid;
        public Transform indicatorFollow;

        private NPCConfig _npcConfig;
        public NPCConfig NpcConfig
        {
            get => _npcConfig;
            set
            {
                _npcConfig = value;
                SetName(_npcConfig.Name);
            }
        }

        private NPCOverrideConfig _overrideConfig;
        public NPCOverrideConfig OverrideConfig
        {
            get => _overrideConfig;
            set
            {
                _overrideConfig = value;
                if (value != null && value.Name != null)
                {
                    SetName(value.Name);
                }
                else
                {
                    SetName(_npcConfig.Name);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            NpcConfig = ConfigMgr.tb.TbNPCConfig[npcCid];
        }

        public override void Interact(Transform interactor)
        {
            UIMgr.Instance.PushPanel<DialoguePanel>().Set(
                OverrideConfig?.DialogueCid ?? _npcConfig.DialogueConfigId,interactor, transform);
        }
    }
}