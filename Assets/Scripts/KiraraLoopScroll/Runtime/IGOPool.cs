using UnityEngine;

namespace KiraraLoopScroll
{
    public interface IGOPool
    {
        public GameObject GetObject(int index);
        public void ReturnObject(GameObject go);
    }
}