using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;

namespace Kirara
{
    public class ParticleMgr : UnitySingleton<ParticleMgr>
    {
        private readonly Dictionary<GameObject, Stack<GameObject>> particlePool = new();

        private async UniTaskVoid ScheduleRecycle(Stack<GameObject> pool, GameObject go, float duration)
        {
            await UniTask.WaitForSeconds(duration);
            go.SetActive(false);
            go.transform.parent = transform;
            pool.Push(go);
        }

        private GameObject PoolGet(GameObject prefab, out Stack<GameObject> pool)
        {
            if (particlePool.TryGetValue(prefab, out pool))
            {
                if (pool.Count > 0)
                {
                    return pool.Pop();
                }
                return Instantiate(prefab, transform);
            }
            pool = new Stack<GameObject>();
            particlePool.Add(prefab, pool);
            return Instantiate(prefab, transform);
        }

        public ParticleSystem Play(string location, Vector3 worldPos)
        {
            var handle = AssetMgr.Instance.package.LoadAssetSync<GameObject>(location);
            var ps =  Play(handle.AssetObject as GameObject, worldPos);
            handle.Release();
            return ps;
        }

        public ParticleSystem Play(GameObject prefab, Vector3 worldPos)
        {
            var go = PoolGet(prefab, out var pool);
            go.transform.position = worldPos;
            go.SetActive(true);
            var particle = go.GetComponent<ParticleSystem>();
            particle.Play();
            ScheduleRecycle(pool, go, particle.main.duration).Forget();
            return particle;
        }

        public ParticleSystem Play(GameObject prefab, Vector3 worldPos, Vector3 forward,
            float rotMinDeg, float rotMaxDeg)
        {
            var go = PoolGet(prefab, out var pool);
            go.transform.position = worldPos;
            go.SetActive(true);
            var particle = go.GetComponent<ParticleSystem>();

            var particleCtrl = go.GetComponent<ParticleCtrl>();
            if (particleCtrl != null)
            {
                particleCtrl.Set(forward, rotMinDeg, rotMaxDeg);
            }
            else
            {
                Debug.LogWarning("particleCtrl == null");
            }

            particle.Play();
            ScheduleRecycle(pool, go, particle.main.duration).Forget();
            return particle;
        }

        public ParticleSystem Play(GameObject prefab, Transform parent)
        {
            var go = PoolGet(prefab, out var pool);
            go.transform.parent = parent;
            go.SetActive(true);
            var particle = go.GetComponent<ParticleSystem>();
            particle.Play();
            ScheduleRecycle(pool, go, particle.main.duration).Forget();
            return particle;
        }
    }
}