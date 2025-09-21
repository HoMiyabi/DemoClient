using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class ExchangePanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button           UIBackBtn;
        private KiraraLoopScroll.GridScrollView ScrollView;
        private UnityEngine.CanvasGroup         CanvasGroup;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn   = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            ScrollView  = b.Q<KiraraLoopScroll.GridScrollView>(1, "ScrollView");
            CanvasGroup = b.Q<UnityEngine.CanvasGroup>(2, "CanvasGroup");
        }
        #endregion

        [SerializeField] private GameObject ExchangeItemPrefab;

        private List<NExchangeItem> items;

        protected override void Awake()
        {
            base.Awake();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            ScrollView.SetSource(new LoopScrollGOPool(ExchangeItemPrefab, transform));
            ScrollView.provideData = ProvideData;

            UpdateItems().Forget();
        }

        private async UniTaskVoid UpdateItems()
        {
            var rsp = await NetFn.ReqGetExchangeItems(new ReqGetExchangeItems());
            items = rsp.Items.ToList();
            ScrollView._totalCount = items.Count;
            ScrollView.RefreshToStart();
        }

        private void ProvideData(GameObject go, int idx)
        {
            var item = items[idx];
            go.GetComponent<UIExchangeItem>()
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
                        UIMgr.Instance.PushPanel<ObtainPanel>()
                            .Set(item.ToConfigId, dialog.Value * item.ToCount);
                    });
                });
        }
    }
}