using cfg.main;
using Kirara.Model;
using Kirara.Service;

namespace Kirara.Quest
{
    public class GatherQuest : ProgressQuest
    {
        public readonly GatherQuestConfig config;
        protected override ProgressQuestConfig progressQuestConfig => config;

        public GatherQuest(GatherQuestConfig config, QuestChain questChain): base(questChain)
        {
            this.config = config;
        }

        public override void Enable()
        {
            base.Enable();
            InventoryService.OnObtainItem += OnObtainItem;
        }

        public override void Disable()
        {
            base.Disable();
            InventoryService.OnObtainItem -= OnObtainItem;
        }

        private void OnObtainItem(BaseItem item, int count)
        {
            if (item is MaterialItem mat)
            {
                if (mat.Cid == config.ItemCid)
                {
                    Progress += count;
                    if (Progress >= Count)
                    {
                        questChain.CompleteQuest(this);
                    }
                }
            }
        }
    }
}