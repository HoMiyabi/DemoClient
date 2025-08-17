using cfg.main;
using Kirara.AttrAbility;
using Manager;

namespace Kirara.Model
{
    public class MonsterModel
    {
        public int MonsterId { get; private set; }
        public MonsterConfig Config { get; private set; }

        public AttrAbilitySet Set { get; } = new();

        public MonsterModel(int monsterCid, int monsterId, float hp)
        {
            MonsterId = monsterId;

            Config = ConfigMgr.tb.TbMonsterConfig[monsterCid];
            Set[EAttrType.Atk] = Config.Atk;
            Set[EAttrType.Def] = Config.Def;
            Set[EAttrType.Hp] = Config.Hp;
            Set[EAttrType.MaxDaze] = Config.MaxDaze;
            Set[EAttrType.StunDuration] = Config.StunDuration;
            Set[EAttrType.StunDmgMultiplier] = Config.StunDmgMultiplier;

            Set[EAttrType.CurrHp] = hp;
            Set[EAttrType.CurrDaze] = 0f;
        }
    }
}