using System;
using System.Collections.Generic;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using XLua;

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

        private List<AbstractBasePanel> stk;


        public int NormalCount => stk.Count;

        protected override void Awake()
        {
            base.Awake();

            canvass = new[] { hudCanvas, normalCanvas, topCanvas };

            foreach (var canvas in canvass)
            {
                DontDestroyOnLoad(canvas.gameObject);
            }

            stk = new List<AbstractBasePanel>();
        }

        private T Init<T>(GameObject go) where T : AbstractBasePanel
        {
            var panel = go.GetComponent<T>();

            if (stk.Count > 0)
            {
                stk[^1].OnPause();
            }
            stk.Add(panel);

            panel.OnResume();
            panel.PlayEnter();
            return panel;
        }

        public T PushPanel<T>(GameObject prefab) where T : AbstractBasePanel
        {
            var go = Instantiate(prefab, canvass[(int)UILayer.Normal]);
            return Init<T>(go);
        }

        public T PushPanel<T>() where T : AbstractBasePanel
        {
            var go = LoadInLayer(typeof(T).Name, UILayer.Normal);
            return Init<T>(go);
        }

        public AbstractBasePanel PushPanel(string location)
        {
            var go = LoadInLayer(location, UILayer.Normal);
            return Init<AbstractBasePanel>(go);
        }

        public AbstractBasePanel PushPanel(Type type)
        {
            return PushPanel(type.Name);
        }

        public void PopPanel(AbstractBasePanel panel)
        {
            if (stk.Count == 0 || stk[^1] != panel)
            {
                Debug.LogError($"PopPanel，但不在{nameof(stk)}中");
                return;
            }
            stk.RemoveAt(stk.Count - 1);

            panel.OnPause();
            panel.onPlayExitFinished += () =>
            {
                panel.onClosed?.Invoke();
                Destroy(panel.gameObject);
            };
            panel.PlayExit();

            if (stk.Count > 0)
            {
                stk[^1].OnResume();
            }
        }

        public void PopAllPanel()
        {
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

        private GameObject LoadInLayer(string location, UILayer layer)
        {
            var handle = AssetMgr.Instance.package.LoadAssetSync<GameObject>(location);
            var go = handle.InstantiateSync(canvass[(int)layer]);
            handle.Release();
            return go;
        }

        private T AddView<T>(UILayer layer)
        {
            string location = typeof(T).Name;
            var go = LoadInLayer(location, layer);
            return go.GetComponent<T>();
        }
    }
}