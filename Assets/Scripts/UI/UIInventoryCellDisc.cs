using System.Linq;
using Kirara;
using Kirara.Model;
using Kirara.UI;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

public class UIInventoryCellDisc : MonoBehaviour, ISelectItem
{
    #region View
    private TextMeshProUGUI InfoText;
    private Image WearerIconImg;
    private Image IconImg;
    private Button Btn;
    private UIInventoryRankBar UIInventoryRankBar;
    private UIDiscPosIcon UIDiscPosIcon;
    private Image SelectBorder;
    private void InitUI()
    {
        var c = GetComponent<KiraraRuntimeComponents>();
        c.Init();
        InfoText = c.Q<TextMeshProUGUI>("InfoText");
        WearerIconImg = c.Q<Image>("WearerIconImg");
        IconImg = c.Q<Image>("IconImg");
        Btn = c.Q<Button>("Btn");
        UIInventoryRankBar = c.Q<UIInventoryRankBar>("UIInventoryRankBar");
        UIDiscPosIcon = c.Q<UIDiscPosIcon>("UIDiscPosIcon");
        SelectBorder = c.Q<Image>("SelectBorder");
    }
    #endregion

    private AssetHandle itemIconHandle;
    private AssetHandle wearerIconHandle;
    private AssetHandle posIconHandle;
    private SelectController selectController;

    private void Awake()
    {
        InitUI();
    }

    private void OnDestroy()
    {
        Clear();
    }

    public void Clear()
    {
        if (_disc != null)
        {
            _disc.OnRoleIdChanged -= UpdateRole;
            _disc.OnLevelChanged -= UpdateLevel;
        }

        itemIconHandle?.Release();
        itemIconHandle = null;
        wearerIconHandle?.Release();
        wearerIconHandle = null;
        posIconHandle?.Release();
        posIconHandle = null;
    }

    private DiscItem _disc;

    public DiscItem Disc
    {
        set
        {
            if (_disc == value) return;
            Clear();
            _disc = value;
            _disc.OnRoleIdChanged += UpdateRole;
            _disc.OnLevelChanged += UpdateLevel;

            UpdateRole();
            UpdateLevel();

            SetIcon(_disc.IconLoc);
            UIInventoryRankBar.Set(_disc.Rank);
            UIDiscPosIcon.Set(_disc.Pos);
        }
    }

    private void UpdateLevel()
    {
        InfoText.text = $"等级{_disc.Level}";
    }

    public void Set(DiscItem disc, UnityAction onClick)
    {
        Disc = disc;
        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(onClick);
    }

    private void UpdateRole()
    {
        if (_disc.RoleId == null)
        {
            WearerIconImg.sprite = null;
            WearerIconImg.gameObject.SetActive(false);
            return;
        }
        WearerIconImg.gameObject.SetActive(true);
        var chInfo = PlayerService.player.Roles.First(it => it.Id == _disc.RoleId);
        wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(chInfo.config.IconLoc);
        WearerIconImg.sprite = wearerIconHandle.AssetObject as Sprite;
    }

    private void SetIcon(string iconLocation)
    {
        itemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(iconLocation);
        IconImg.sprite = itemIconHandle.AssetObject as Sprite;
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