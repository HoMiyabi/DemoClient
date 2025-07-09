using cfg.main;
using Kirara.AttrEffect;
using Manager;

namespace Kirara.Model
{
    public class MonsterModel
    {
        public int MonsterId { get; private set; }
        public int MonsterCid { get; private set; }

        public AttrSet AttrSet { get; set; } = new();

        public MonsterModel(int monsterCid, int monsterId)
        {
            MonsterId = monsterId;
            MonsterCid = monsterCid;

            var config = ConfigMgr.tb.TbMonsterConfig[monsterCid];
            AttrSet[EAttrType.Atk] = config.Atk;
            AttrSet[EAttrType.Def] = config.Def;
            AttrSet[EAttrType.Hp] = config.Hp;
            AttrSet[EAttrType.MaxDaze] = config.MaxDaze;
            AttrSet[EAttrType.StunDuration] = config.StunDuration;
            AttrSet[EAttrType.StunDmgMultiplier] = config.StunDmgMultiplier;

            AttrSet[EAttrType.CurrHp] = config.Hp;
            AttrSet[EAttrType.CurrDaze] = 0f;
        }
    }
}