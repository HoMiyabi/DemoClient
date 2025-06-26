using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class SimpleLoopScrollPrefabSource : MonoBehaviour, LoopScrollPrefabSource
    {
        [SerializeField]
        private GameObject prefab;

        private readonly Stack<GameObject> pool = new();

        public GameObject GetObject(int index)
        {
            if (pool.Count == 0)
            {
                return Instantiate(prefab);
            }
            var go = pool.Pop();
            go.SetActive(true);
            return go;
        }

        public void ReturnObject(Transform trans)
        {
            trans.SetParent(transform, false);
            var go = trans.gameObject;
            go.SetActive(false);
            pool.Push(go);
        }
    }
}