using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class ExchangePanel : BasePanel
    {
        private Button UIBackBtn;
        private LoopVerticalScrollRect ExchangeLoopScroll;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.dict["UIBackBtn"] as Button;
            ExchangeLoopScroll = c.dict["ExchangeLoopScroll"] as LoopVerticalScrollRect;
        }

        private AssetHandle cellHandle;

        private Stack<Transform> pool;
        private List<NExchangeItem> items;

        private void Awake()
        {
            InitUI();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            pool = new Stack<Transform>();
            cellHandle = AssetMgr.Instance.package.LoadAssetSync<GameObject>("UIExchangeCell");
        }

        public void Clear()
        {
            cellHandle?.Release();
            cellHandle = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Start()
        {
            var scrollHandler = new UILoopScrollHandler(ScrollGetObject, ScrollReturnObject, ScrollProvideData);

            ExchangeLoopScroll.prefabSource = scrollHandler;
            ExchangeLoopScroll.dataSource = scrollHandler;
            UpdateItems().Forget();
        }

        private async UniTaskVoid UpdateItems()
        {
            var rsp = await NetFn.ReqGetExchangeItems(new ReqGetExchangeItems());
            items = rsp.Items.ToList();
            ExchangeLoopScroll.totalCount = items.Count;
            ExchangeLoopScroll.RefillCells();
        }

        private GameObject ScrollGetObject(int _)
        {
            if (pool.Count == 0)
            {
                return Instantiate(cellHandle.AssetObject as GameObject);
            }
            var trans = pool.Pop();
            trans.gameObject.SetActive(true);
            return trans.gameObject;
        }

        private void ScrollReturnObject(Transform trans)
        {
            trans.GetComponent<UIExchangeCell>().Clear();
            trans.gameObject.SetActive(false);
            pool.Push(trans);
        }

        private void ScrollProvideData(Transform trans, int idx)
        {
            var item = items[idx];
            trans.GetComponent<UIExchangeCell>()
                .Set(item)
                .OnClick(() =>
                {
                    var dialog = UIMgr.Instance.PushPanel<ExchangeDialogPanel>()
                        .Set(item);
                    dialog.Confirmed = UniTask.UnityAction(async () =>
                    {
                        UIMgr.Instance.PopPanel(dialog);
                        var rsp = await NetFn.ReqExchange(new ReqExchange
                        {
                            ExchangeId = item.ExchangeId,
                            ExchangeCount = dialog.Value,
                        });
                        if (rsp.Code == 0)
                        {
                            Debug.Log("购买成功");
                            UIMgr.Instance.PushPanel<ObtainPanel>()
                                .Set(item.ToConfigId, dialog.Value * item.ToCount);
                        }
                        else
                        {
                            Debug.LogWarning($"购买失败: {rsp.Msg}");
                        }
                    });
                });
        }
    }
}