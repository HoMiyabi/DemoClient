using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class LoopScrollDataSourceHandler : LoopScrollDataSource
    {
        private readonly Action<Transform, int> provideData;

        public LoopScrollDataSourceHandler(Action<Transform, int> provideData)
        {
            this.provideData = provideData;
        }

        public void ProvideData(Transform transform, int idx)
        {
            provideData?.Invoke(transform, idx);
        }
    }
}