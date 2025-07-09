using System;
using System.Collections.Generic;
using Kirara.Model;
using Kirara.UI;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kirara
{
    public class MonsterSystem : UnitySingleton<MonsterSystem>
    {
        [SerializeField] private Transform monsterParent;
        public readonly Dictionary<int, Monster> monsters = new();

        public List<Monster> AttackingMonsters { get; private set; } = new();

        public event Action<Monster> OnMonsterSpawn;
        public event Action<Monster> OnMonsterDie;

        public void SpawnMonster(int monsterCid, int monsterId, Vector3 pos, Quaternion rot)
        {
            var config = ConfigMgr.tb.TbMonsterConfig[monsterCid];

            var handle = AssetMgr.Instance.package.LoadAssetSync<GameObject>(config.Location);
            var go = handle.InstantiateSync(monsterParent);
            handle.Release();

            go.GetComponent<CharacterController>().enabled = false;
            go.transform.position = pos;
            go.GetComponent<CharacterController>().enabled = true;
            go.transform.rotation = rot;

            var monster = go.GetComponent<Monster>();
            monster.Set(new MonsterModel(monsterCid, monsterId));
            monsters.Add(monsterId, monster);

            UIMgr.Instance.AddHUD<UIMonsterStatusBar>().Set(monster);

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

        public Monster ClosestMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, monsters.Values, out dist);
        }

        public Monster ClosestAttackingMonster(Vector3 worldPos, out float dist)
        {
            return ClosestMonster(worldPos, AttackingMonsters, out dist);
        }

        private static Monster ClosestMonster(Vector3 worldPos, IEnumerable<Monster> monsters, out float dist)
        {
            Monster ret = null;
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