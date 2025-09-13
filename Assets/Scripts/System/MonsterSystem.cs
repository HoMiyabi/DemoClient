using System;
using System.Collections.Generic;
using Kirara.Model;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara
{
    public class MonsterSystem : UnitySingleton<MonsterSystem>
    {
        [SerializeField] private Transform monsterParent;
        public readonly Dictionary<int, MonsterCtrl> monsterCtrls = new();

        public List<MonsterCtrl> DodgeDetectMonsters { get; } = new();

        public event Action<MonsterCtrl> OnMonsterSpawn;
        public event Action<MonsterCtrl> OnMonsterDie;

        public delegate void OnMonsterTakeDamageDel(MonsterCtrl monsterCtrl, double damage, bool isCrit);
        public event OnMonsterTakeDamageDel OnMonsterTakeDamage;

        public delegate void OnMonsterAttackTipDel(MonsterCtrl monsterCtrl, bool canParry);
        public event OnMonsterAttackTipDel OnMonsterAttackTip;

        public void MonsterTakeDamage(NotifyMonsterTakeDamage msg)
        {
            if (monsterCtrls.TryGetValue(msg.MonsterId, out var monsterCtrl))
            {
                monsterCtrl.HandleSelfTakeDamage(msg);
                OnMonsterTakeDamage?.Invoke(monsterCtrl, msg.Damage, msg.IsCrit);
            }
        }

        public void SpawnMonster(NSyncMonster syncMonster)
        {
            var config = ConfigMgr.tb.TbMonsterConfig[syncMonster.MonsterCid];

            var handle = YooAssets.LoadAssetSync<GameObject>(config.Location);
            var go = handle.InstantiateSync(monsterParent);

            // go.GetComponent<CharacterController>().enabled = false;
            go.transform.position = syncMonster.Pos.Unity();
            // go.GetComponent<CharacterController>().enabled = true;
            go.transform.rotation = syncMonster.Rot.Unity();

            var monster = go.GetComponent<MonsterCtrl>();
            monster.Set(new MonsterModel(syncMonster.MonsterCid, syncMonster.MonsterId, syncMonster.Hp));
            monsterCtrls.Add(syncMonster.MonsterCid, monster);
            if (!string.IsNullOrEmpty(syncMonster.ActionName))
            {
                monster.PlayAction(syncMonster.ActionName);
            }


            // UIMgr.Instance.AddHUD<UIMonsterStatusBar>().Set(monster);

            OnMonsterSpawn?.Invoke(monster);
        }

        public void MonsterDie(int monsterId)
        {
            if (monsterCtrls.Remove(monsterId, out var monster))
            {
                OnMonsterDie?.Invoke(monster);
                monster.HandleSelfDie();
            }
            else
            {
                Debug.LogWarning($"MonsterDie找不到Monster monsterId={monsterId}");
            }
        }

        public void DoAttackTip(MonsterCtrl monsterCtrl, bool canParry)
        {
            OnMonsterAttackTip?.Invoke(monsterCtrl, canParry);
        }

        public MonsterCtrl ClosestMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, monsterCtrls.Values, out dist);
        }

        public MonsterCtrl ClosestDodgeDetectMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, DodgeDetectMonsters, out dist);
        }

        private static MonsterCtrl ClosestMonster(Vector3 worldPos, IEnumerable<MonsterCtrl> monsters, out float dist)
        {
            MonsterCtrl ret = null;
            dist = float.MaxValue;
            foreach (var monster in monsters)
            {
                float d = Vector3.Distance(worldPos, monster.transform.position);
                if (d < dist)
                {
                    ret = monster;
                    dist = d;
                }
            }
            return ret;
        }
    }
}