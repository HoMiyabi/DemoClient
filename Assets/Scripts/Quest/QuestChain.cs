using System;
using System.Collections.Generic;
using cfg.main;
using Manager;
using UnityEngine;

namespace Kirara.Quest
{
    public class QuestChain
    {
        private readonly QuestChainConfig config;

        public int Cid => config.QuestChainConfigId;
        public string Name => config.Name;
        public List<RewordConfig> Rewords => config.Rewords;


        private Quest _quest;
        public Quest Quest
        {
            get => _quest;
            private set
            {
                _quest = value;
                OnQuestSet?.Invoke();
            }
        }
        public Action OnQuestSet;

        public QuestChain(int questChainCid, int questCid)
        {
            config = ConfigMgr.tb.TbQuestChainConfig[questChainCid];

            var questConfig = config.Quests.Find(questConfig =>
                questConfig.QuestCid == questCid);

            Quest = questConfig.BuildRuntime(this);
            Quest.Enable();
        }

        public void OnTrack()
        {
            Quest?.OnTrack();
        }

        public void OnStopTrack()
        {
            Quest?.OnStopTrack();
        }

        public bool IsTracking => QuestSystem.Instance.TrackingChain == this;

        public void CompleteQuest(Quest quest)
        {
            if (Quest != quest)
            {
                Debug.LogWarning($"quest is not current quest");
                return;
            }
            CompleteQuest();
        }

        public void CompleteQuest()
        {
            Quest.Disable();
            Quest.OnStopTrack();
            Quest.OnComplete();

            int nextQuestCid = Quest.NextQuestCid;
            if (nextQuestCid != 0)
            {
                // 进行下一个任务
                var nextQuestConfig = config.Quests
                    .Find(questConfig => questConfig.QuestCid == nextQuestCid);
                if (nextQuestConfig != null)
                {
                    Quest = nextQuestConfig.BuildRuntime(this);
                    Quest.Enable();
                    if (IsTracking)
                    {
                        Quest.OnTrack();
                    }
                    QuestSystem.Instance.SendStartQuest(this);
                }
                else
                {
                    Debug.LogError($"没有下一个任务 nextQuestCid={nextQuestCid}");
                }
            }
            else
            {
                Quest = null;
                // 任务链完成
                QuestSystem.Instance.CompleteQuestChain(this);
            }
        }
    }
}