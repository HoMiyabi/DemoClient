using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;
using Manager;
using TMPro;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class UpgradeDiscDialogPanel : BasePanel
    {
        #region View
        private Button              UICloseBtn;
        private UIDiscPosIcon       UIDiscPosIcon;
        private UIStatBar           UIMainStatBar;
        private UIStatBar           UISubStatBar1;
        private UIStatBar           UISubStatBar2;
        private UIStatBar           UISubStatBar3;
        private UIStatBar           UISubStatBar4;
        private Button              UpgradeBtn;
        private Button              UIUpgradeMaterial;
        private Button              DecBtn;
        private TextMeshProUGUI     CountText;
        private UIDiscIcon          UIDiscIcon;
        private UIDiscNameText      UIDiscNameText;
        private UIUpgradeDiscExpBar UIUpgradeDiscExpBar;
        private void InitUI()
        {
            var c               = GetComponent<KiraraRuntimeComponents>();
            UICloseBtn          = c.Q<Button>(0, "UICloseBtn");
            UIDiscPosIcon       = c.Q<UIDiscPosIcon>(1, "UIDiscPosIcon");
            UIMainStatBar       = c.Q<UIStatBar>(2, "UIMainStatBar");
            UISubStatBar1       = c.Q<UIStatBar>(3, "UISubStatBar1");
            UISubStatBar2       = c.Q<UIStatBar>(4, "UISubStatBar2");
            UISubStatBar3       = c.Q<UIStatBar>(5, "UISubStatBar3");
            UISubStatBar4       = c.Q<UIStatBar>(6, "UISubStatBar4");
            UpgradeBtn          = c.Q<Button>(7, "UpgradeBtn");
            UIUpgradeMaterial   = c.Q<Button>(8, "UIUpgradeMaterial");
            DecBtn              = c.Q<Button>(9, "DecBtn");
            CountText           = c.Q<TextMeshProUGUI>(10, "CountText");
            UIDiscIcon          = c.Q<UIDiscIcon>(11, "UIDiscIcon");
            UIDiscNameText      = c.Q<UIDiscNameText>(12, "UIDiscNameText");
            UIUpgradeDiscExpBar = c.Q<UIUpgradeDiscExpBar>(13, "UIUpgradeDiscExpBar");
        }
        #endregion

        private UIStatBar[] subStatBars;

        private DiscItem disc;
        private AssetHandle discHandle;

        private int _useCount;
        public int UseCount
        {
            get => _useCount;
            set
            {
                _useCount = value;
                UpdateDecBtnView();
                UpdateCountTextView();
                UpdateUpgradeBtnView();
                UIUpgradeDiscExpBar.SetAddExp(value * mat.Exp);
            }
        }

        private MaterialItem mat;
        private int MatCount => mat?.Count ?? 0;

        private void Awake()
        {
            InitUI();

            int matCid = 1;

            mat = PlayerService.Player.Materials.Find(it => it.Cid == matCid);

            UICloseBtn.onClick.AddListener(PopPanel);
            subStatBars = new[] {UISubStatBar1, UISubStatBar2, UISubStatBar3, UISubStatBar4};

            UIUpgradeMaterial.onClick.AddListener(UIUpgradeMaterial_onClick);
            DecBtn.onClick.AddListener(DecBtn_onClick);
            UpgradeBtn.onClick.AddListener(UpgradeBtn_onClick);
        }

        private void Clear()
        {
            if (disc != null)
            {
                disc.OnSubAttrsChanged -= UpdateSubAttrsView;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Set(DiscItem disc)
        {
            Clear();
            this.disc = disc;

            UIUpgradeDiscExpBar.Set(disc);
            UseCount = 0;

            UIDiscIcon.Set(disc.IconLoc); // 图标
            UIDiscPosIcon.Set(disc.Pos); // 位置
            UIDiscNameText.Set(disc.Name, disc.Pos); // 名字

            // 主属性
            UIMainStatBar.Set(disc.MainAttr);
            // 副属性
            UpdateSubAttrsView();
            disc.OnSubAttrsChanged += UpdateSubAttrsView;
        }

        private void UpdateSubAttrsView()
        {
            for (int i = 0; i < subStatBars.Length; i++)
            {
                if (i < disc.SubAttrs.Count)
                {
                    subStatBars[i].gameObject.SetActive(true);
                    subStatBars[i].Set(disc.SubAttrs[i]);
                }
                else
                {
                    subStatBars[i].gameObject.SetActive(false);
                }
            }
        }

        private void UIUpgradeMaterial_onClick()
        {
            if (UseCount < MatCount)
            {
                UseCount++;
            }
        }

        private void DecBtn_onClick()
        {
            if (UseCount >= 1)
            {
                UseCount--;
            }
        }

        private void UpgradeBtn_onClick()
        {
            DiscService.Upgrade(disc, mat.Cid, mat.Exp, UseCount).Forget();
            UseCount = 0;
        }

        private void UpdateDecBtnView()
        {
            DecBtn.gameObject.SetActive(UseCount >= 1);
        }

        private void UpdateCountTextView()
        {
            CountText.text = $"{UseCount}/{MatCount}";
        }

        private void UpdateUpgradeBtnView()
        {
            UpgradeBtn.interactable = UseCount >= 1;
        }
    }
}