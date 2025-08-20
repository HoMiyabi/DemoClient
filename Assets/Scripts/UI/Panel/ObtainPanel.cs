using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class ObtainPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button ConfirmBtn;
        private TMPro.TextMeshProUGUI ToItemCountText;
        private UnityEngine.UI.Image  Icon;
        private UnityEngine.UI.Button UIOverlayBtn;
        private TMPro.TextMeshProUGUI NameText;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            ConfirmBtn      = b.Q<UnityEngine.UI.Button>(0, "ConfirmBtn");
            ToItemCountText = b.Q<TMPro.TextMeshProUGUI>(1, "ToItemCountText");
            Icon            = b.Q<UnityEngine.UI.Image>(2, "Icon");
            UIOverlayBtn    = b.Q<UnityEngine.UI.Button>(3, "UIOverlayBtn");
            NameText        = b.Q<TMPro.TextMeshProUGUI>(4, "NameText");
        }
        #endregion

        private AssetHandle iconHandle;

        protected override void Awake()
        {
            base.Awake();

            UIOverlayBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            ConfirmBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        private void Clear()
        {
            iconHandle?.Release();
            iconHandle = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public ObtainPanel Set(int weaponCid, int count)
        {
            Clear();

            var config = ConfigMgr.tb.TbWeaponConfig[weaponCid];
            iconHandle = YooAssets.LoadAssetSync<Sprite>(config.IconLoc);
            Icon.sprite = iconHandle.AssetObject as Sprite;
            NameText.text = config.Name;

            ToItemCountText.text = $"{count}";

            return this;
        }
    }
}