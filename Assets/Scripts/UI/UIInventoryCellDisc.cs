using System.Linq;
using Kirara;
using Kirara.Model;
using Manager;
using UnityEngine;

public class UIInventoryCellDisc : MonoBehaviour
{
    #region View
    private bool _isBound;
    private TMPro.TextMeshProUGUI        InfoText;
    private UnityEngine.UI.Image         WearerIconImg;
    private UnityEngine.UI.Image         IconImg;
    private UnityEngine.UI.Button        Btn;
    private Kirara.UI.UIInventoryRankBar UIInventoryRankBar;
    private Kirara.UI.UIDiscPosIcon      UIDiscPosIcon;
    private UnityEngine.UI.Image         SelectBorder;
    public void BindUI()
    {
        if (_isBound) return;
        _isBound = true;
        var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
        InfoText           = c.Q<TMPro.TextMeshProUGUI>(0, "InfoText");
        WearerIconImg      = c.Q<UnityEngine.UI.Image>(1, "WearerIconImg");
        IconImg            = c.Q<UnityEngine.UI.Image>(2, "IconImg");
        Btn                = c.Q<UnityEngine.UI.Button>(3, "Btn");
        UIInventoryRankBar = c.Q<Kirara.UI.UIInventoryRankBar>(4, "UIInventoryRankBar");
        UIDiscPosIcon      = c.Q<Kirara.UI.UIDiscPosIcon>(5, "UIDiscPosIcon");
        SelectBorder       = c.Q<UnityEngine.UI.Image>(6, "SelectBorder");
    }
    #endregion

    private LiveData<DiscItem> _selected;

    private void OnDestroy()
    {
        Clear();
    }

    private void Clear()
    {
        if (_disc != null)
        {
            _disc.OnRoleIdChanged -= UpdateRole;
            _disc.OnLevelChanged -= UpdateLevel;
            _disc = null;
        }
        _selected?.Remove(OnSelectionChanged);
        _disc = null;
    }

    private DiscItem _disc;

    public DiscItem Disc
    {
        set
        {
            Clear();
            _disc = value;
            _disc.OnRoleIdChanged += UpdateRole;
            _disc.OnLevelChanged += UpdateLevel;

            UpdateRole();
            UpdateLevel();

            SetIcon(_disc.IconLoc);
            UIInventoryRankBar.Set(_disc.Config.Rank);
            UIDiscPosIcon.Set(_disc.Pos);
        }
    }

    private void UpdateLevel()
    {
        InfoText.text = $"等级{_disc.Level}";
    }

    public void Set(DiscItem disc, LiveData<DiscItem> selected)
    {
        BindUI();
        Clear();

        Disc = disc;
        _selected = selected;
        _selected.Observe(OnSelectionChanged);
        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(Btn_onClick);
    }

    private void UpdateRole()
    {
        if (string.IsNullOrEmpty(_disc.RoleId))
        {
            WearerIconImg.sprite = null;
            WearerIconImg.gameObject.SetActive(false);
        }
        else
        {
            WearerIconImg.gameObject.SetActive(true);
            var role = PlayerService.Player.Roles.First(it => it.Id == _disc.RoleId);
            var wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.Config.IconLoc);
            WearerIconImg.sprite = wearerIconHandle.AssetObject as Sprite;
        }
    }

    private void SetIcon(string iconLocation)
    {
        var itemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(iconLocation);
        IconImg.sprite = itemIconHandle.AssetObject as Sprite;
    }

    private void OnSelectionChanged(DiscItem disc)
    {
        if (disc == _disc)
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
        _selected?.Set(_disc);
    }
}