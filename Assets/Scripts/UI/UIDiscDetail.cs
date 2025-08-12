using System.Linq;
using Kirara.Model;
using Manager;
using UnityEngine;

namespace Kirara.UI
{
    public class UIDiscDetail : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI   NameText;
        private UnityEngine.UI.Image    WearerIcon;
        private TMPro.TextMeshProUGUI   LevelText;
        private UnityEngine.UI.Image    Icon;
        private Kirara.UI.UIStatBar     MainAttrBar;
        private TMPro.TextMeshProUGUI   EffDescContentText;
        private Kirara.UI.UIStatBar     SubAttrBar;
        private Kirara.UI.UIStatBar     SubAttrBar1;
        private Kirara.UI.UIStatBar     SubAttrBar2;
        private Kirara.UI.UIStatBar     SubAttrBar3;
        private UnityEngine.UI.Image    BackIcon;
        private Kirara.UI.UIDiscPosIcon UIDiscPosIcon;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            NameText           = c.Q<TMPro.TextMeshProUGUI>(0, "NameText");
            WearerIcon         = c.Q<UnityEngine.UI.Image>(1, "WearerIcon");
            LevelText          = c.Q<TMPro.TextMeshProUGUI>(2, "LevelText");
            Icon               = c.Q<UnityEngine.UI.Image>(3, "Icon");
            MainAttrBar        = c.Q<Kirara.UI.UIStatBar>(4, "MainAttrBar");
            EffDescContentText = c.Q<TMPro.TextMeshProUGUI>(5, "EffDescContentText");
            SubAttrBar         = c.Q<Kirara.UI.UIStatBar>(6, "SubAttrBar");
            SubAttrBar1        = c.Q<Kirara.UI.UIStatBar>(7, "SubAttrBar1");
            SubAttrBar2        = c.Q<Kirara.UI.UIStatBar>(8, "SubAttrBar2");
            SubAttrBar3        = c.Q<Kirara.UI.UIStatBar>(9, "SubAttrBar3");
            BackIcon           = c.Q<UnityEngine.UI.Image>(10, "BackIcon");
            UIDiscPosIcon      = c.Q<Kirara.UI.UIDiscPosIcon>(11, "UIDiscPosIcon");
        }
        #endregion

        private DiscItem _disc;

        private UIStatBar[] subAttrBars;

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            if (_disc != null)
            {
                _disc.OnLevelChanged -= UpdateLevelView;
                _disc.OnSubAttrsChanged -= UpdateSubAttrsView;
            }
        }

        public UIDiscDetail Set(DiscItem disc)
        {
            BindUI();
            if (subAttrBars == null)
            {
                subAttrBars = new[] { SubAttrBar, SubAttrBar1, SubAttrBar2, SubAttrBar3 };
            }

            Clear();
            _disc = disc;

            NameText.text = $"{disc.Name}[{disc.Pos}]";
            UpdateLevelView();
            disc.OnLevelChanged += UpdateLevelView;

            var discIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(disc.IconLoc);
            var discSprite = discIconHandle.AssetObject as Sprite;

            BackIcon.sprite = discSprite;
            Icon.sprite = discSprite;

            MainAttrBar.Set(disc.MainAttr);
            UpdateSubAttrsView();
            disc.OnSubAttrsChanged += UpdateSubAttrsView;

            EffDescContentText.text = $"2件套: {disc.Config.SetAbility2Desc}\n" +
                                      $"4件套: {disc.Config.SetAbility4Desc}";

            UIDiscPosIcon.Set(disc.Pos);
            SetRole(disc.RoleId);

            return this;
        }

        private void UpdateSubAttrsView()
        {
            for (int i = 0; i < subAttrBars.Length; i++)
            {
                if (i < _disc.SubAttrs.Count)
                {
                    subAttrBars[i].gameObject.SetActive(true);
                    subAttrBars[i].Set(_disc.SubAttrs[i]);
                }
                else
                {
                    subAttrBars[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateLevelView()
        {
            LevelText.text = $"等级{_disc.Level}/{ConfigMgr.tb.TbGlobalConfig.DiscMaxLevel}";
        }

        private void SetRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                WearerIcon.gameObject.SetActive(false);
                return;
            }
            var role = PlayerService.Player.Roles.First(it => it.Id == roleId);
            var wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.Config.IconLoc);
            WearerIcon.sprite = wearerIconHandle.AssetObject as Sprite;
        }
    }
}