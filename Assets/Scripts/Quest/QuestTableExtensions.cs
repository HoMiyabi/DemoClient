using System.Linq;
using cfg.main;
using UnityEngine;

namespace Kirara.Quest
{
    public static class QuestTableExtensions
    {
        // public static QuestChain GetRuntime(this QuestChainConfig config)
        // {
        //     return new QuestChain()
        // }

        public static Quest BuildRuntime(this QuestConfig config, QuestChain questChain)
        {
            switch (config.GetTypeId())
            {
                case DialogueQuestConfig.__ID__: return new DialogueQuest(config as DialogueQuestConfig, questChain);
                case GotoQuestConfig.__ID__: return new GotoQuest(config as GotoQuestConfig, questChain);
                case DefeatQuestConfig.__ID__: return new DefeatQuest(config as DefeatQuestConfig, questChain);
                case GatherQuestConfig.__ID__: return new GatherQuest(config as GatherQuestConfig, questChain);
                case UpgradeQuestConfig.__ID__: return new UpgradeQuest(config as UpgradeQuestConfig, questChain);
            }
            Debug.LogError("config type not found");
            return null;
        }
    }
}