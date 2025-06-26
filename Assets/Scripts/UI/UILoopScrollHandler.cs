using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public struct UILoopScrollHandler : LoopScrollPrefabSource, LoopScrollDataSource
    {
        public Func<int, GameObject> getObject;
        public Action<Transform> returnObject;
        public Action<Transform, int> provideData;

        public UILoopScrollHandler(Func<int, GameObject> getObject, Action<Transform> returnObject, Action<Transform, int> provideData)
        {
            this.getObject = getObject;
            this.returnObject = returnObject;
            this.provideData = provideData;
        }

        public GameObject GetObject(int index) => getObject(index);
        public void ReturnObject(Transform trans) => returnObject(trans);
        public void ProvideData(Transform transform, int idx) => provideData.Invoke(transform, idx);
    }
}