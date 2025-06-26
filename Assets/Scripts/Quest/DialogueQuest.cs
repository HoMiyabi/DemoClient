using System.Collections.Generic;
using cfg.main;
using Kirara.System;

namespace Kirara.Quest
{
    public class DialogueQuest : Quest
    {
        public readonly DialogueQuestConfig config;
        protected override QuestConfig BaseQuestConfig => config;

        public DialogueQuest(DialogueQuestConfig config, QuestChain questChain) : base(questChain)
        {
            this.config = config;
        }

        public override void Enable()
        {
            base.Enable();
            DialogueSystem.Instance.OnDialogueFinish += OnDialogueFinish;
        }

        public override void Disable()
        {
            base.Disable();
            DialogueSystem.Instance.OnDialogueFinish -= OnDialogueFinish;
        }

        private void OnDialogueFinish(int dialogueCid, Dictionary<string, int> blackBoard)
        {
            const string failed = "failed";
            if (dialogueCid == config.DialogueCid)
            {
                if (blackBoard.GetValueOrDefault(failed) == 0)
                {
                    questChain.CompleteQuest(this);
                }
            }
        }
    }
}