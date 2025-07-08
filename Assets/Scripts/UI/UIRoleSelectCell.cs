using cfg.main;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIRoleSelectCell : MonoBehaviour, ISelectItem
    {
        #region View
        private Image  SelectBorder;
        private Image  Icon;
        private Button Btn;
        private void InitUI()
        {
            var c        = GetComponent<KiraraRuntimeComponents>();
            SelectBorder = c.Q<Image>(0, "SelectBorder");
            Icon         = c.Q<Image>(1, "Icon");
            Btn          = c.Q<Button>(2, "Btn");
        }
        #endregion

        private AssetHandle handle;
        private SelectController selectController;
        private Bindable<int> _selected;
        private int _idx;

        private void Awake()
        {
            InitUI();
        }

        private void Clear()
        {
            handle?.Release();
            if (_selected != null)
            {
                _selected.OnValueChanged -= OnValueChanged;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Set(CharacterConfig chConfig, int idx, Bindable<int> selected)
        {
            Clear();
            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(chConfig.RoleSelectIconLoc);
            Icon.sprite = handle.AssetObject as Sprite;

            _selected = selected;
            _idx = idx;
            _selected.OnValueChanged += OnValueChanged;
            OnValueChanged(_selected.Value);

            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(Btn_onClick);
        }

        private void OnValueChanged(int idx)
        {
            if (idx == _idx)
            {
                OnSelect();
            }
            else
            {
                OnDeselect();
            }
        }

        private void Btn_onClick()
        {
            _selected.Value = _idx;
        }

        public void OnSelect()
        {
            SelectBorder.gameObject.SetActive(true);
        }

        public void OnDeselect()
        {
            SelectBorder.gameObject.SetActive(false);
        }
    }
}