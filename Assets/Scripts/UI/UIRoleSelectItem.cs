using cfg.main;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIRoleSelectItem : MonoBehaviour, ISelectItem
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image  SelectBorder;
        private UnityEngine.UI.Image  Icon;
        private UnityEngine.UI.Button Btn;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c        = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SelectBorder = c.Q<UnityEngine.UI.Image>(0, "SelectBorder");
            Icon         = c.Q<UnityEngine.UI.Image>(1, "Icon");
            Btn          = c.Q<UnityEngine.UI.Button>(2, "Btn");
        }
        #endregion

        private AssetHandle handle;
        private SelectController selectController;
        private Bindable<int> _selected;
        private int _idx;

        private void Awake()
        {
            BindUI();
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

        public void Set(RoleConfig roleConfig, int idx, Bindable<int> selected)
        {
            Clear();
            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(roleConfig.RoleSelectIconLoc);
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