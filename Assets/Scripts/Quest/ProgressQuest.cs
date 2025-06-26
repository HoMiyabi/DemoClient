using System;
using cfg.main;

namespace Kirara.Quest
{
    public abstract class ProgressQuest : Quest
    {
        protected int progress;
        public int Progress
        {
            get => progress;
            protected set
            {
                if (progress == value) return;
                progress = value;
                OnProgressChanged?.Invoke();
            }
        }
        public event Action OnProgressChanged;

        public int Count => progressQuestConfig.Count;
        protected abstract ProgressQuestConfig progressQuestConfig { get; }
        protected override QuestConfig BaseQuestConfig => progressQuestConfig;

        protected ProgressQuest(QuestChain questChain) : base(questChain)
        {
        }
    }
}