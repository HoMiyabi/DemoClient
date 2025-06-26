using System.Collections.Generic;
using cfg.main;
using Kirara.Indicator;
using Manager;

namespace Kirara.Quest
{
    public class DefeatQuest : ProgressQuest
    {
		public readonly DefeatQuestConfig config;
        protected override ProgressQuestConfig progressQuestConfig => config;

        private readonly List<Monster> monsters = new();

        public DefeatQuest(DefeatQuestConfig config, QuestChain questChain): base(questChain)
        {
            this.config = config;
        }

        public override void Enable()
        {
            base.Enable();
            MonsterSystem.Instance.OnMonsterDie += OnMonsterDie;
            MonsterSystem.Instance.OnMonsterSpawn += OnMonsterSpawn;
        }

        private void OnMonsterSpawn(Monster monster)
        {
            if (monster.monsterCid != config.MonsterCid) return;

            if (monsters.Count + Progress < config.Count)
            {
                monsters.Add(monster);
                if (QuestSystem.Instance.TrackingChain == questChain)
                {
                    IndicatorSystem.AddIndicator(
                        PlayerSystem.Instance.Player.transform,
                        monster.transform);
                }
            }
        }

        public override void Disable()
        {
            base.Disable();
            MonsterSystem.Instance.OnMonsterDie -= OnMonsterDie;
            MonsterSystem.Instance.OnMonsterSpawn -= OnMonsterSpawn;
        }

        public override void OnTrack()
        {
            base.OnTrack();

            foreach (var monster in monsters)
            {
                IndicatorSystem.AddIndicator(
                    PlayerSystem.Instance.Player.transform,
                    monster.indicatorFollow);
            }
        }

        private void OnMonsterDie(Monster monster)
        {
            if (monster.monsterCid != config.MonsterCid) return;

            if (monsters.Remove(monster))
            {
                Progress++;
                if (Progress >= config.Count)
                {
                    questChain.CompleteQuest(this);
                }
            }
        }
    }
}