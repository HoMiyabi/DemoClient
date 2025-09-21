using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kirara.Model;
using Kirara.Service;
using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class UpgradeDiscDialogPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button         UICloseBtn;
        private Kirara.UI.UIDiscPosIcon       UIDiscPosIcon;
        private Kirara.UI.UIStatBar           UIMainStatBar;
        private Kirara.UI.UIStatBar           UISubStatBar1;
        private Kirara.UI.UIStatBar           UISubStatBar2;
        private Kirara.UI.UIStatBar           UISubStatBar3;
        private Kirara.UI.UIStatBar           UISubStatBar4;
        private UnityEngine.UI.Button         UpgradeBtn;
        private UnityEngine.UI.Button         UIUpgradeMaterial;
        private UnityEngine.UI.Button         DecBtn;
        private TMPro.TextMeshProUGUI         CountText;
        private Kirara.UI.UIDiscIcon          UIDiscIcon;
        private Kirara.UI.UIDiscNameText      UIDiscNameText;
        private Kirara.UI.UIUpgradeDiscExpBar UIUpgradeDiscExpBar;
        private UnityEngine.CanvasGroup       CanvasGroup;
        private UnityEngine.RectTransform     Box;
        public override void BindUI()
        {
        if (_isBound) return;
        _isBound = true;
            var b               = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UICloseBtn          = b.Q<UnityEngine.UI.Button>(0, "UICloseBtn");
            UIDiscPosIcon       = b.Q<Kirara.UI.UIDiscPosIcon>(1, "UIDiscPosIcon");
            UIMainStatBar       = b.Q<Kirara.UI.UIStatBar>(2, "UIMainStatBar");
            UISubStatBar1       = b.Q<Kirara.UI.UIStatBar>(3, "UISubStatBar1");
            UISubStatBar2       = b.Q<Kirara.UI.UIStatBar>(4, "UISubStatBar2");
            UISubStatBar3       = b.Q<Kirara.UI.UIStatBar>(5, "UISubStatBar3");
            UISubStatBar4       = b.Q<Kirara.UI.UIStatBar>(6, "UISubStatBar4");
            UpgradeBtn          = b.Q<UnityEngine.UI.Button>(7, "UpgradeBtn");
            UIUpgradeMaterial   = b.Q<UnityEngine.UI.Button>(8, "UIUpgradeMaterial");
            DecBtn              = b.Q<UnityEngine.UI.Button>(9, "DecBtn");
            CountText           = b.Q<TMPro.TextMeshProUGUI>(10, "CountText");
            UIDiscIcon          = b.Q<Kirara.UI.UIDiscIcon>(11, "UIDiscIcon");
            UIDiscNameText      = b.Q<Kirara.UI.UIDiscNameText>(12, "UIDiscNameText");
            UIUpgradeDiscExpBar = b.Q<Kirara.UI.UIUpgradeDiscExpBar>(13, "UIUpgradeDiscExpBar");
            CanvasGroup         = b.Q<UnityEngine.CanvasGroup>(14, "CanvasGroup");
            Box                 = b.Q<UnityEngine.RectTransform>(15, "Box");
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

        protected override void Awake()
        {
            base.Awake();

            int matCid = 1;

            mat = PlayerService.Player.Materials.Find(it => it.Cid == matCid);

            UICloseBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
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

        public override void PlayEnter()
        {
            Box.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Box.DOScale(1f, 0.1f);
            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        public override void PlayExit()
        {
            Box.DOScale(0.8f, 0.1f);
            CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
        }
    }
}