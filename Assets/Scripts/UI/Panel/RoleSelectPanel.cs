using System.Collections.Generic;
using cfg.main;
using DG.Tweening;
using Kirara;
using Kirara.UI;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;

public class RoleSelectPanel : BasePanel
{
    #region View
    private bool _isBound;
    private UnityEngine.UI.Button           UIBackBtn;
    private UnityEngine.UI.Button           SelectBtn;
    private KiraraLoopScroll.GridScrollView LoopScroll;
    private UnityEngine.CanvasGroup         CanvasGroup;
    public override void BindUI()
    {
        if (_isBound) return;
        _isBound = true;
        var b       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
        UIBackBtn   = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
        SelectBtn   = b.Q<UnityEngine.UI.Button>(1, "SelectBtn");
        LoopScroll  = b.Q<KiraraLoopScroll.GridScrollView>(2, "LoopScroll");
        CanvasGroup = b.Q<UnityEngine.CanvasGroup>(3, "CanvasGroup");
    }
    #endregion

    public GameObject RoleSelectItemPrefab;
    public float offset;
    private readonly LiveData<int> selected = new(0);
    private List<RoleConfig> list;

    protected override void Awake()
    {
        base.Awake();

        list = ConfigMgr.tb.TbRoleConfig.DataList;

        UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        SelectBtn.onClick.AddListener(() =>
        {
            var roleConfig = list[selected.Value];
            var role = PlayerService.Player.Roles.Find(x => x.Config.Id == roleConfig.Id);
            if (role == null)
            {
                return;
            }
            UIMgr.Instance.PushPanel<RoleDetailPanel>().Set(role);
        });

        LoopScroll._totalCount = list.Count;
        LoopScroll.SetSource(new LoopScrollGOPool(RoleSelectItemPrefab, transform));
        LoopScroll.provideData = ProvideData;
        LoopScroll.updateItem = UpdateItem;
    }

    private void UpdateItem(RectTransform item, int index)
    {
        float y = item.anchoredPosition.y;
        float x = item.anchoredPosition.x;
        if ((index % 3 + 3) % 3 == 1)
        {
            y -= offset;
        }
        x += -y * Mathf.Tan(16 * Mathf.Deg2Rad);
        item.anchoredPosition = new Vector2(x, y);
    }

    private void ProvideData(GameObject go, int idx)
    {
        var cell = go.GetComponent<UIRoleSelectItem>();
        int i = (idx % list.Count + list.Count) % list.Count;
        var config = list[i];
        cell.Set(config, i, selected);
    }

    public override void PlayEnter()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
    }

    public override void PlayExit()
    {
        CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
    }
}