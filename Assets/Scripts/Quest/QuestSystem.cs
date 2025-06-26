using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara;
using Kirara.Manager;
using Kirara.Quest;
using UnityEngine;

namespace Manager
{
    public class QuestSystem : UnitySingleton<QuestSystem>
    {
        // 当前获得的任务链
        public List<QuestChain> chains;

        // 当前追踪的任务链
        private QuestChain trackingChain;
        public QuestChain TrackingChain
        {
            get => trackingChain;
            set
            {
                if (trackingChain == value) return;

                trackingChain?.OnStopTrack();
                trackingChain = value;
                trackingChain?.OnTrack();

                OnTrackingChainChanged?.Invoke();
            }
        }
        public event Action OnTrackingChainChanged;

        private void Start()
        {
            Init(PlayerService.player.questProgresses);
        }

        public void Init(List<(int questChainCid, int currentQuestCid)> questProgresses)
        {
            chains = new List<QuestChain>(questProgresses.Count);

            foreach ((int questChainCid, int currentQuestCid) in questProgresses)
            {
                AddQuestChain(questChainCid, currentQuestCid);
            }
        }

        public void EnterRoom()
        {
            foreach (var questChain in chains)
            {
                SendStartQuest(questChain).Forget();
            }
        }

        public async UniTaskVoid SendStartQuest(QuestChain questChain)
        {
            var req = new ReqStartQuest
            {
                QuestChainCid = questChain.Cid,
                QuestCid = questChain.Quest.Cid
            };
            var rsp = await NetFn.ReqStartQuest(req);
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"StartQuest failed: {rsp.Msg}");
            }
        }

        public void AddQuestChain(int questChainCid, int questCid, bool truck = false)
        {
            var questChain = new QuestChain(questChainCid, questCid);

            chains.Add(questChain);
            if (truck)
            {
                TrackingChain = questChain;
            }
        }

        public void CompleteQuestChain(QuestChain chain)
        {
            if (TrackingChain == chain)
            {
                TrackingChain = null;
            }
            if (!chains.Remove(chain))
            {
                Debug.LogWarning($"Remove Failed {chain.Cid}");
            }
            var msg = new MsgCompleteQuestChain
            {
                QuestChainCid = chain.Cid
            };
            NetFn.Send(msg);
        }
    }
}