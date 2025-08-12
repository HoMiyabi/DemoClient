using System.Collections.Generic;
using cfg.main;
using Kirara;
using Kirara.UI;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;

public class RoleSelectPanel : BasePanel
{
    #region View
    private bool _isBound;
    private UnityEngine.UI.Button UIBackBtn;
    private UnityEngine.UI.Button SelectBtn;
    private KiraraLoopScroll.GridScrollView        LoopScroll;
    private SelectController      SelectController;
    public override void BindUI()
    {
        if (_isBound) return;
        _isBound = true;
        var c            = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
        UIBackBtn        = c.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
        SelectBtn        = c.Q<UnityEngine.UI.Button>(1, "SelectBtn");
        LoopScroll       = c.Q<KiraraLoopScroll.GridScrollView>(2, "LoopScroll");
        SelectController = c.Q<SelectController>(3, "SelectController");
    }
    #endregion

    public GameObject RoleSelectCellPrefab;
    public float offset;
    private readonly Stack<GameObject> pool = new();
    private readonly Bindable<int> selected = new(0);
    private List<CharacterConfig> list;

    protected override void Awake()
    {
        base.Awake();

        list = ConfigMgr.tb.TbCharacterConfig.DataList;

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

        LoopScroll.totalCount = list.Count;
        LoopScroll.SetPoolFunc(GetObject, ReturnObject);
        LoopScroll.provideData = ProvideData;
        LoopScroll.updateCell = UpdatePos;
    }

    private void UpdatePos(RectTransform rectTransform, int arg2)
    {
        float y = rectTransform.anchoredPosition.y;
        float x = rectTransform.anchoredPosition.x;
        if (MathUtils.Repeat(arg2, 3) == 1)
        {
            y -= offset;
        }
        x += -y * Mathf.Tan(16 * Mathf.Deg2Rad);
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    private void ProvideData(GameObject go, int idx)
    {
        var cell = go.GetComponent<UIRoleSelectCell>();
        var list = ConfigMgr.tb.TbCharacterConfig.DataList;
        int i = (idx % list.Count + list.Count) % list.Count;
        var config = list[i];
        cell.Set(config, i, selected);
    }

    private GameObject GetObject(int idx)
    {
        if (pool.Count == 0)
        {
            return Instantiate(RoleSelectCellPrefab);
        }
        var go = pool.Pop();
        go.SetActive(true);
        return go;
    }

    private void ReturnObject(GameObject go)
    {
        go.transform.SetParent(transform, false);
        go.SetActive(false);
        pool.Push(go);
    }
}