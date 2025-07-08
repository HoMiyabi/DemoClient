using System.Linq;
using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIDiscDetail : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI NameText;
        private Image           WearerIcon;
        private TextMeshProUGUI LevelText;
        private Image           Icon;
        private UIStatBar       MainAttrBar;
        private TextMeshProUGUI EffDescContentText;
        private UIStatBar       SubAttrBar;
        private UIStatBar       SubAttrBar1;
        private UIStatBar       SubAttrBar2;
        private UIStatBar       SubAttrBar3;
        private Image           BackIcon;
        private UIDiscPosIcon   UIDiscPosIcon;
        private void InitUI()
        {
            var c              = GetComponent<KiraraRuntimeComponents>();
            NameText           = c.Q<TextMeshProUGUI>(0, "NameText");
            WearerIcon         = c.Q<Image>(1, "WearerIcon");
            LevelText          = c.Q<TextMeshProUGUI>(2, "LevelText");
            Icon               = c.Q<Image>(3, "Icon");
            MainAttrBar        = c.Q<UIStatBar>(4, "MainAttrBar");
            EffDescContentText = c.Q<TextMeshProUGUI>(5, "EffDescContentText");
            SubAttrBar         = c.Q<UIStatBar>(6, "SubAttrBar");
            SubAttrBar1        = c.Q<UIStatBar>(7, "SubAttrBar1");
            SubAttrBar2        = c.Q<UIStatBar>(8, "SubAttrBar2");
            SubAttrBar3        = c.Q<UIStatBar>(9, "SubAttrBar3");
            BackIcon           = c.Q<Image>(10, "BackIcon");
            UIDiscPosIcon      = c.Q<UIDiscPosIcon>(11, "UIDiscPosIcon");
        }
        #endregion

        private DiscItem disc;
        private AssetHandle discIconHandle;
        private AssetHandle posIconHandle;
        private AssetHandle wearerIconHandle;

        private UIStatBar[] subAttrBars;

        private void Awake()
        {
            InitUI();
            subAttrBars = new[] { SubAttrBar, SubAttrBar1, SubAttrBar2, SubAttrBar3 };
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            if (disc != null)
            {
                disc.OnLevelChanged -= UpdateLevelView;
                disc.OnSubAttrsChanged -= UpdateSubAttrsView;
            }
            discIconHandle?.Release();
            discIconHandle = null;
            posIconHandle?.Release();
            posIconHandle = null;
            wearerIconHandle?.Release();
            wearerIconHandle = null;
        }

        public UIDiscDetail Set(DiscItem disc)
        {
            Clear();
            this.disc = disc;

            NameText.text = $"{disc.Name}[{disc.Pos}]";
            UpdateLevelView();
            disc.OnLevelChanged += UpdateLevelView;

            discIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(disc.IconLoc);
            var discSprite = discIconHandle.AssetObject as Sprite;

            BackIcon.sprite = discSprite;
            Icon.sprite = discSprite;

            MainAttrBar.Set(disc.MainAttr);
            UpdateSubAttrsView();
            disc.OnSubAttrsChanged += UpdateSubAttrsView;

            EffDescContentText.text = $"2件套: {disc.Config.SetEffect2Desc}\n" +
                                      $"4件套: {disc.Config.SetEffect4Desc}";

            UIDiscPosIcon.Set(disc.Pos);
            SetRole(disc.RoleId);

            return this;
        }

        private void UpdateSubAttrsView()
        {
            for (int i = 0; i < subAttrBars.Length; i++)
            {
                if (i < disc.SubAttrs.Count)
                {
                    subAttrBars[i].gameObject.SetActive(true);
                    subAttrBars[i].Set(disc.SubAttrs[i]);
                }
                else
                {
                    subAttrBars[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateLevelView()
        {
            LevelText.text = $"等级{disc.Level}/{ConfigMgr.tb.TbGlobalConfig.DiscMaxLevel}";
        }

        private void SetRole(string roleId)
        {
            if (roleId == null)
            {
                WearerIcon.gameObject.SetActive(false);
                return;
            }
            var chInfo = PlayerService.Player.Roles.First(it => it.Id == roleId);
            wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(chInfo.config.IconLoc);
            WearerIcon.sprite = wearerIconHandle.AssetObject as Sprite;
        }
    }
}