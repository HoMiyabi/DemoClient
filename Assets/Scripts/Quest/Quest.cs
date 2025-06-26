using System;
using System.Collections.Generic;
using cfg.main;
using Kirara.Indicator;
using Kirara.UI;
using Manager;

namespace Kirara.Quest
{
    public abstract class Quest
    {
        public readonly QuestChain questChain;
        protected abstract QuestConfig BaseQuestConfig { get; }

        public int Cid => BaseQuestConfig.QuestCid;
        public int NextQuestCid => BaseQuestConfig.NextQuestCid;
        public string Name => BaseQuestConfig.Name;
        public string Desc => BaseQuestConfig.Desc;
        public List<NPCOverrideConfig> NPCOverrides => BaseQuestConfig.NpcOverrides;

        public bool HasNextQuest => BaseQuestConfig.NextQuestCid != 0;
        public List<int> TruckNpcCids => BaseQuestConfig.TrunkNpcCids;

        protected Quest(QuestChain questChain)
        {
            this.questChain = questChain;
        }

        public virtual void Enable()
        {
            foreach (var npcOverride in NPCOverrides)
            {
                NPCSystem.Instance.npcDict[npcOverride.NpcCid].OverrideConfig = npcOverride;
            }
        }

        public virtual void Disable()
        {
            foreach (var npcOverride in NPCOverrides)
            {
                NPCSystem.Instance.npcDict[npcOverride.NpcCid].OverrideConfig = null;
            }
        }

        public virtual void OnComplete()
        {
            if (BaseQuestConfig.CompleteGetQuestChainCid != 0)
            {
                QuestSystem.Instance.AddQuestChain(BaseQuestConfig.CompleteGetQuestChainCid, 1, true);
            }
            UIMgr.Instance.AddTop<UINotification>().Set(Name + " 完成");
        }

        public virtual void OnTrack()
        {
            foreach (int cid in TruckNpcCids)
            {
                IndicatorSystem.AddIndicator(
                    PlayerSystem.Instance.Player.transform,
                    NPCSystem.Instance.npcDict[cid].indicatorFollow);
            }
        }

        public virtual void OnStopTrack()
        {
            IndicatorSystem.ClearIndicators();
        }
    }
}