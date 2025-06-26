using System.Collections.Generic;
using UnityEngine;

namespace Kirara
{
    public class SimpleGOPool
    {
        private readonly GameObject prefab;
        private readonly Transform poolPlace;
        private readonly Stack<GameObject> pool;

        public SimpleGOPool(GameObject prefab, Transform poolPlace)
        {
            this.prefab = prefab;
            this.poolPlace = poolPlace;

            pool = new Stack<GameObject>();
        }

        public GameObject Get()
        {
            if (pool.Count > 0)
            {
                var go = pool.Pop();
                go.SetActive(true);
                return go;
            }
            return Object.Instantiate(prefab);
        }

        public T Get<T>() where T : Component
        {
            return Get().GetComponent<T>();
        }

        public T Get<T>(Transform parent, bool worldPositionStays) where T : Component
        {
            var com = Get().GetComponent<T>();
            com.transform.SetParent(parent, worldPositionStays);
            return com;
        }

        public void Release(GameObject go)
        {
            go.transform.SetParent(poolPlace, false);
            go.SetActive(false);
            pool.Push(go);
        }

        public void ReleaseChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Release(parent.GetChild(i).gameObject);
            }
        }
    }
}