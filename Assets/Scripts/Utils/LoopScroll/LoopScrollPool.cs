using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara
{
    public class LoopScrollPool : LoopScrollPrefabSource
    {
        private readonly GameObject prefab;
        private readonly Transform poolPlace;
        private readonly Stack<GameObject> pool = new();
        private readonly int max;

        public LoopScrollPool(GameObject prefab, Transform poolPlace, int max = 128)
        {
            this.prefab = prefab;
            this.poolPlace = poolPlace;
            this.max = max;
        }

        public GameObject GetObject(int index)
        {
            if (pool.Count > 0)
            {
                var go = pool.Pop();
                go.SetActive(true);
                return go;
            }
            return Object.Instantiate(prefab);
        }

        public void ReturnObject(Transform trans)
        {
            if (pool.Count < max)
            {
                trans.SetParent(poolPlace, false);
                var go = trans.gameObject;
                go.SetActive(false);
                pool.Push(go);
            }
            else
            {
                Object.Destroy(trans.gameObject);
            }
        }
    }
}