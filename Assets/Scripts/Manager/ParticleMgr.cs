using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Kirara
{
    public class ParticleMgr : UnitySingleton<ParticleMgr>
    {
        private readonly Dictionary<GameObject, Stack<GameObject>> particlePool = new();
        private int maxPoolCount = 64;

        private async UniTaskVoid ScheduleRecycle(Stack<GameObject> pool, GameObject go, float duration)
        {
            await UniTask.WaitForSeconds(duration);
            // Debug.Log($"回收粒子, name: {go.name}");
            if (pool.Count < maxPoolCount)
            {
                go.SetActive(false);
                go.transform.parent = transform;
                pool.Push(go);
            }
            else
            {
                Destroy(go);
            }
        }

        private void GetGameObjectAndPool(GameObject prefab, out GameObject go, out Stack<GameObject> pool)
        {
            if (particlePool.TryGetValue(prefab, out pool))
            {
                if (pool.Count > 0)
                {
                    go = pool.Pop();
                }
                else
                {
                    go = Instantiate(prefab, transform);
                }
            }
            else
            {
                pool = new Stack<GameObject>();
                particlePool.Add(prefab, pool);
                go = Instantiate(prefab, transform);
            }
        }

        public ParticleSystem PlayAt(string location, Vector3 worldPosition)
        {
            var handle = YooAssets.LoadAssetSync<GameObject>(location);
            var ps =  PlayAt(handle.AssetObject as GameObject, worldPosition);
            handle.Release();
            return ps;
        }

        public ParticleSystem PlayAt(GameObject prefab, Vector3 worldPosition)
        {
            GetGameObjectAndPool(prefab, out var go, out var pool);

            go.transform.position = worldPosition;
            go.SetActive(true);
            var particle = go.GetComponent<ParticleSystem>();
            particle.Play();
            ScheduleRecycle(pool, go, particle.main.duration).Forget();

            return particle;
        }

        public ParticleSystem PlayAt(GameObject prefab, Vector3 worldPosition, Vector3 forward,
            float rotMinDeg, float rotMaxDeg)
        {
            GetGameObjectAndPool(prefab, out var go, out var pool);

            go.transform.position = worldPosition;
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

        public ParticleSystem PlayAsChild(GameObject prefab, Transform parent,
            Vector3 localPosition , Quaternion localRotation, Vector3 localScale)
        {
            GetGameObjectAndPool(prefab, out var go, out var pool);

            go.transform.parent = parent;
            go.SetActive(true);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = localRotation;
            go.transform.localScale = localScale;

            var particle = go.GetComponent<ParticleSystem>();
            particle.Play();
            ScheduleRecycle(pool, go, particle.main.duration).Forget();

            return particle;
        }
    }
}