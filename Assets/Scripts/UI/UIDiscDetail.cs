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
        private Image WearerIcon;
        private TextMeshProUGUI LevelText;
        private Image Icon;
        private UIStatBar MainAttrBar;
        private TextMeshProUGUI EffDescContentText;
        private UIStatBar SubAttrBar;
        private UIStatBar SubAttrBar1;
        private UIStatBar SubAttrBar2;
        private UIStatBar SubAttrBar3;
        private Image BackIcon;
        private UIDiscPosIcon UIDiscPosIcon;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            NameText = c.Q<TextMeshProUGUI>("NameText");
            WearerIcon = c.Q<Image>("WearerIcon");
            LevelText = c.Q<TextMeshProUGUI>("LevelText");
            Icon = c.Q<Image>("Icon");
            MainAttrBar = c.Q<UIStatBar>("MainAttrBar");
            EffDescContentText = c.Q<TextMeshProUGUI>("EffDescContentText");
            SubAttrBar = c.Q<UIStatBar>("SubAttrBar");
            SubAttrBar1 = c.Q<UIStatBar>("SubAttrBar1");
            SubAttrBar2 = c.Q<UIStatBar>("SubAttrBar2");
            SubAttrBar3 = c.Q<UIStatBar>("SubAttrBar3");
            BackIcon = c.Q<Image>("BackIcon");
            UIDiscPosIcon = c.Q<UIDiscPosIcon>("UIDiscPosIcon");
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