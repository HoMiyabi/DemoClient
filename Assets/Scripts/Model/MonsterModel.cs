using cfg.main;
using Kirara.AttrAbility;
using Manager;

namespace Kirara.Model
{
    public class MonsterModel
    {
        public int MonsterId { get; private set; }
        public int MonsterCid { get; private set; }

        public AttrAbilitySet Set { get; } = new();

        public MonsterModel(int monsterCid, int monsterId, float hp)
        {
            MonsterId = monsterId;
            MonsterCid = monsterCid;

            var config = ConfigMgr.tb.TbMonsterConfig[monsterCid];
            Set[EAttrType.Atk] = config.Atk;
            Set[EAttrType.Def] = config.Def;
            Set[EAttrType.Hp] = config.Hp;
            Set[EAttrType.MaxDaze] = config.MaxDaze;
            Set[EAttrType.StunDuration] = config.StunDuration;
            Set[EAttrType.StunDmgMultiplier] = config.StunDmgMultiplier;

            Set[EAttrType.CurrHp] = hp;
            Set[EAttrType.CurrDaze] = 0f;
        }
    }
}