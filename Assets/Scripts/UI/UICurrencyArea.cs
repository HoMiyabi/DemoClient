using System.Linq;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UICurrencyArea : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI CountText;
        private UnityEngine.UI.Image  CurrencyIcon;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b        = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            CountText    = b.Q<TMPro.TextMeshProUGUI>(0, "CountText");
            CurrencyIcon = b.Q<UnityEngine.UI.Image>(1, "CurrencyIcon");
        }
        #endregion

        public void Set(int currencyCid)
        {
            BindUI();

            var itemConfig = ConfigMgr.tb.TbCurrencyItemConfig[currencyCid];
            var handle = YooAssets.LoadAssetSync<Sprite>(itemConfig.IconLoc);
            CurrencyIcon.sprite = handle.AssetObject as Sprite;
            handle.Release();

            var item = PlayerService.Player.Currencies.FirstOrDefault(x => x.Cid == currencyCid);
            int count = item?.Count ?? 0;
            CountText.text = count.ToString();
        }
    }
}