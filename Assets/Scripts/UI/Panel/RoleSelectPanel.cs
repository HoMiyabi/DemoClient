using System.Collections.Generic;
using Kirara;
using Kirara.UI;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class RoleSelectPanel : BasePanel
{
    #region View
    private Button UIBackBtn;
    private Button SelectBtn;
    private GridScroller LoopScroll;
    private SelectController SelectController;
    private void InitUI()
    {
        var c = GetComponent<KiraraRuntimeComponents>();
        c.Init();
        UIBackBtn = c.Q<Button>("UIBackBtn");
        SelectBtn = c.Q<Button>("SelectBtn");
        LoopScroll = c.Q<GridScroller>("LoopScroll");
        SelectController = c.Q<SelectController>("SelectController");
    }
    #endregion

    public GameObject RoleSelectCellPrefab;
    public float offset;
    private readonly Stack<GameObject> pool = new();
    private readonly Bindable<int> selected = new(0);

    private void Awake()
    {
        InitUI();

        UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

        var list = ConfigMgr.tb.TbCharacterConfig.DataList;

        LoopScroll.totalCount = list.Count;
        LoopScroll.SetPoolFunc(GetObject, ReturnObject);
        LoopScroll.provideData = ProvideData;
        LoopScroll.updateCell = UpdatePos;
        Init();
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

    private void Init()
    {
        // foreach (var character in PlayerSystem.Instance.ChCtrls)
        // {
        //     var go = handle.InstantiateSync(Content);
        //     go.GetComponent<SelectCharacterCell>().Set(character.ChModel, () =>
        //     {
        //         UIMgr.Instance.PushPanel<CharacterDetailPanel>().Set(character.ChModel);
        //     });
        // }
    }
}