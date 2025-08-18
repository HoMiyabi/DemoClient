using UnityEngine;

namespace KiraraLoopScroll
{
    public interface IGameObjectSource
    {
        public GameObject GetObject(int index);
        public void ReturnObject(GameObject go);
    }
}