using System;
using System.Collections.Generic;
using Kirara.Model;
using Kirara.UI;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara
{
    public class MonsterSystem : UnitySingleton<MonsterSystem>
    {
        [SerializeField] private Transform monsterParent;
        public readonly Dictionary<int, MonsterCtrl> monsters = new();

        public List<MonsterCtrl> AttackingMonsters { get; private set; } = new();

        public event Action<MonsterCtrl> OnMonsterSpawn;
        public event Action<MonsterCtrl> OnMonsterDie;

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
            monsters.Add(syncMonster.MonsterCid, monster);
            if (!string.IsNullOrEmpty(syncMonster.ActionName))
            {
                monster.PlayAction(syncMonster.ActionName);
            }


            // UIMgr.Instance.AddHUD<UIMonsterStatusBar>().Set(monster);

            OnMonsterSpawn?.Invoke(monster);
        }

        public void MonsterDie(int monsterId)
        {
            if (monsters.Remove(monsterId, out var monster))
            {
                OnMonsterDie?.Invoke(monster);
                monster.Die();
            }
            else
            {
                Debug.LogWarning($"MonsterDie找不到Monster monsterId={monsterId}");
            }
        }

        public MonsterCtrl ClosestMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, monsters.Values, out dist);
        }

        public MonsterCtrl ClosestAttackingMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, AttackingMonsters, out dist);
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