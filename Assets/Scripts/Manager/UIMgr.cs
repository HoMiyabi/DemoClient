using System;
using System.Collections.Generic;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using XLua;
using YooAsset;

namespace Kirara
{
    public enum UILayer
    {
        HUD = 0,
        Normal = 1,
        Top = 2,
    }

    [LuaCallCSharp]
    public class UIMgr : UnitySingleton<UIMgr>
    {
        [SerializeField] private RectTransform hudCanvas;
        [SerializeField] private RectTransform normalCanvas;
        [SerializeField] private RectTransform topCanvas;
        private RectTransform[] canvass;

        private List<AbstractBasePanel> hudStack;
        private List<AbstractBasePanel> normalStack;
        private List<AbstractBasePanel> topStack;
        private List<AbstractBasePanel>[] stacks;

        protected override void Awake()
        {
            base.Awake();

            canvass = new[] { hudCanvas, normalCanvas, topCanvas };

            foreach (var canvas in canvass)
            {
                DontDestroyOnLoad(canvas.gameObject);
            }

            hudStack = new List<AbstractBasePanel>();
            normalStack = new List<AbstractBasePanel>();
            topStack = new List<AbstractBasePanel>();
            stacks = new[] { hudStack, normalStack, topStack };
        }

        private T Init<T>(GameObject go, UILayer layer) where T : AbstractBasePanel
        {
            var panel = go.GetComponent<T>();

            var stk = stacks[(int)layer];
            if (stk.Count > 0)
            {
                stk[^1].OnPause();
            }
            stk.Add(panel);

            panel.OnResume();
            panel.PlayEnter();
            return panel;
        }

        public T PushPanel<T>(GameObject prefab, UILayer layer = UILayer.Normal) where T : AbstractBasePanel
        {
            var go = Instantiate(prefab, canvass[(int)layer]);
            return Init<T>(go, layer);
        }

        public T PushPanel<T>(UILayer layer = UILayer.Normal) where T : AbstractBasePanel
        {
            var go = LoadAssetInLayer(typeof(T).Name, layer);
            return Init<T>(go, layer);
        }

        public AbstractBasePanel PushPanel(string location, UILayer layer = UILayer.Normal)
        {
            var go = LoadAssetInLayer(location, layer);
            return Init<AbstractBasePanel>(go, layer);
        }

        public void PopPanel(AbstractBasePanel panel, UILayer layer = UILayer.Normal)
        {
            var stk = stacks[(int)layer];
            if (stk.Count == 0 || stk[^1] != panel)
            {
                Debug.LogError($"PopPanel，但不在{nameof(stk)}中");
                return;
            }
            stk.RemoveAt(stk.Count - 1);

            panel.OnPause();
            panel.onPlayExitFinished += () =>
            {
                Destroy(panel.gameObject);
            };
            panel.PlayExit();

            if (stk.Count > 0)
            {
                stk[^1].OnResume();
            }
        }

        public void PopAllPanel(UILayer layer = UILayer.Normal)
        {
            var stk = stacks[(int)layer];
            while (stk.Count > 0)
            {
                PopPanel(stk[^1]);
            }
        }

        public T AddHUD<T>()
        {
            return AddView<T>(UILayer.HUD);
        }

        public T AddTop<T>()
        {
            return AddView<T>(UILayer.Top);
        }

        private GameObject LoadAssetInLayer(string location, UILayer layer)
        {
            var handle = YooAssets.LoadAssetSync<GameObject>(location);
            var go = handle.InstantiateSync(canvass[(int)layer]);
            handle.Release();
            return go;
        }

        private T AddView<T>(UILayer layer)
        {
            string location = typeof(T).Name;
            var go = LoadAssetInLayer(location, layer);
            return go.GetComponent<T>();
        }
    }
}