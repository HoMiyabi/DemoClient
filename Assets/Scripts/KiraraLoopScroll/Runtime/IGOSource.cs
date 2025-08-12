using UnityEngine;

namespace KiraraLoopScroll
{
    public interface IGOSource
    {
        public GameObject GetObject(int index);
        public void ReturnObject(GameObject go);
    }
}