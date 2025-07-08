using System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class ObtainPanel : BasePanel
    {
        #region View
        private Button          ConfirmBtn;
        private TextMeshProUGUI ToItemCountText;
        private Image           Icon;
        private Button          UIOverlayBtn;
        private TextMeshProUGUI NameText;
        private void InitUI()
        {
            var c           = GetComponent<KiraraRuntimeComponents>();
            ConfirmBtn      = c.Q<Button>(0, "ConfirmBtn");
            ToItemCountText = c.Q<TextMeshProUGUI>(1, "ToItemCountText");
            Icon            = c.Q<Image>(2, "Icon");
            UIOverlayBtn    = c.Q<Button>(3, "UIOverlayBtn");
            NameText        = c.Q<TextMeshProUGUI>(4, "NameText");
        }
        #endregion

        private AssetHandle iconHandle;

        private void Awake()
        {
            InitUI();

            UIOverlayBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            ConfirmBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public void Clear()
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
            iconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(config.IconLoc);
            Icon.sprite = iconHandle.AssetObject as Sprite;
            NameText.text = config.Name;

            ToItemCountText.text = $"{count}";

            return this;
        }
    }
}