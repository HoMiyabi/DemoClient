using cfg.main;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIRoleSelectItem : MonoBehaviour
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
        private LiveData<int> _selected;
        private int _idx;

        private void Awake()
        {
            BindUI();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
            _selected?.Remove(OnSelectionChanged);
            _selected = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Set(RoleConfig roleConfig, int idx, LiveData<int> selected)
        {
            Clear();
            handle = YooAssets.LoadAssetSync<Sprite>(roleConfig.RoleSelectIconLoc);
            Icon.sprite = handle.AssetObject as Sprite;

            _selected = selected;
            _idx = idx;
            _selected.Observe(OnSelectionChanged);

            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(Btn_onClick);
        }

        private void OnSelectionChanged(int idx)
        {
            if (idx == _idx)
            {
                SelectBorder.gameObject.SetActive(true);
            }
            else
            {
                SelectBorder.gameObject.SetActive(false);
            }
        }

        private void Btn_onClick()
        {
            _selected.Value = _idx;
        }
    }
}