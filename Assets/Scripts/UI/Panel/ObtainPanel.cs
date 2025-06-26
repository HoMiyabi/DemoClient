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
        private Button ConfirmBtn;
        private TextMeshProUGUI ToItemCountText;
        private Image Icon;
        private Button UIOverlayBtn;
        private TextMeshProUGUI NameText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            ConfirmBtn = c.dict["ConfirmBtn"] as Button;
            ToItemCountText = c.dict["ToItemCountText"] as TextMeshProUGUI;
            Icon = c.dict["Icon"] as Image;
            UIOverlayBtn = c.dict["UIOverlayBtn"] as Button;
            NameText = c.dict["NameText"] as TextMeshProUGUI;
        }

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