using cfg.main;
using Kirara.Model;
using Kirara.Service;
using Manager;
using NotImplementedException = System.NotImplementedException;

namespace Kirara.Quest
{
    public class UpgradeQuest : ProgressQuest
    {
        public readonly UpgradeQuestConfig config;
        protected override ProgressQuestConfig progressQuestConfig => config;

        public UpgradeQuest(UpgradeQuestConfig config, QuestChain questChain) : base(questChain)
        {
            this.config = config;
        }

        public override void Enable()
        {
            base.Enable();
            DiscService.OnDiscUpgradedLevel += DiscServiceOnOnDiscUpgradedLevel;
        }

        public override void Disable()
        {
            base.Disable();
            DiscService.OnDiscUpgradedLevel -= DiscServiceOnOnDiscUpgradedLevel;
        }

        private void DiscServiceOnOnDiscUpgradedLevel(DiscItem disc)
        {
            if (disc.Level >= config.TargetLevel)
            {
                Progress++;
                if (Progress >= Count)
                {
                    questChain.CompleteQuest(this);
                }
            }
        }
    }
}