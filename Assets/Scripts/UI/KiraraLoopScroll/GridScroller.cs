using System;
using System.Collections.Generic;
using Kirara.UI;
using UnityEngine;

public class GridScroller : Scroller
{
    public Vector2 size = new(100f, 100f);
    public Vector2 spacing = new(10f, 10f);

    // 裁切时，把当做size + cullingPadding的大小来看
    public Padding cullingPadding;

    public int countInLine = 3;

    // 左闭右开
    private int presentMinIdx;
    private int presentMaxIdx;
    public readonly Dictionary<int, RectTransform> cells = new();
    private readonly Stack<RectTransform> pool = new();

    public Action<RectTransform, int> updateCell;

    private void HidePoolingCells()
    {
        foreach (var cell in pool)
        {
            cell.gameObject.SetActive(false);
        }
    }

    public void Refresh()
    {
        for (int i = presentMinIdx; i < presentMaxIdx; i++)
        {
            ReturnObjectAt(i);
        }
        presentMinIdx = 0;
        presentMaxIdx = 0;
        CullCells();
        UpdateCellsPos();
    }

    private void ReturnObjectAt(int idx)
    {
        if (cells.Remove(idx, out var cell))
        {
            pool.Push(cell);
        }
        else
        {
            Debug.LogError($"{nameof(GridScroller)}: Cell not found for index {idx}");
        }
    }

    private void CheckReturnObjects(int currMinIdx, int currMaxIdx)
    {
        if (currMinIdx > presentMinIdx)
        {
            for (int idx = presentMinIdx; idx < Mathf.Min(currMinIdx, presentMaxIdx); idx++)
            {
                ReturnObjectAt(idx);
            }
        }

        if (currMaxIdx < presentMaxIdx)
        {
            for (int idx = Mathf.Max(currMaxIdx, presentMinIdx); idx < presentMaxIdx; idx++)
            {
                ReturnObjectAt(idx);
            }
        }
    }

    private void GetObjectAt(int idx)
    {
        GameObject go;
        if (pool.Count > 0)
        {
            go = pool.Pop().gameObject;
            go.SetActive(true);
        }
        else
        {
            go = getObject(idx);
        }
        provideData?.Invoke(go, idx);
        go.transform.SetParent(content, false);
        if (!cells.TryAdd(idx, (RectTransform)go.transform))
        {
            Debug.LogError($"KiraraLoopScroll: Cell already exists. Index: {idx}");
        }
    }

    private void CheckGetObjects(int currMinIdx, int currMaxIdx)
    {
        if (currMinIdx < presentMinIdx)
        {
            for (int idx = currMinIdx; idx < Mathf.Min(currMaxIdx, presentMinIdx); idx++)
            {
                GetObjectAt(idx);
            }
        }
        if (currMaxIdx > presentMaxIdx)
        {
            for (int idx = Mathf.Max(currMinIdx, presentMaxIdx); idx < currMaxIdx; idx++)
            {
                GetObjectAt(idx);
            }
        }
    }

    protected override void CullCells()
    {
        int windowMinIdx = WindowMinIdx;
        int windowMaxIdx = WindowMaxIdx;
        CheckReturnObjects(windowMinIdx, windowMaxIdx);
        CheckGetObjects(windowMinIdx, windowMaxIdx);
        HidePoolingCells();
        presentMinIdx = windowMinIdx;
        presentMaxIdx = windowMaxIdx;
    }

    // private int GetIdx(int row, int col)
    // {
    //     return row * countInLine + col;
    // }

    // 窗口内的最小Idx
    private int WindowMinIdx
    {
        get
        {
            int minLine = Mathf.FloorToInt((Pos -  CullingBottom + CullingSpacing) / LineWidth);
            int idx = minLine * countInLine;
            return isInfinite ? idx : Mathf.Clamp(idx, 0, totalCount);
        }
    }

    // 窗口内的最大Idx
    private int WindowMaxIdx
    {
        get
        {
            int maxLine = Mathf.CeilToInt((Pos + WindowLength + CullingTop) / LineWidth);
            int idx = maxLine * countInLine;
            return isInfinite ? idx : Mathf.Clamp(idx, 0, totalCount);
        }
    }

    private float CullingBottom => direction switch
    {
        EDirection.Vertical => cullingPadding.bottom,
        EDirection.Horizontal => cullingPadding.right,
        _ => throw new ArgumentOutOfRangeException()
    };

    private float CullingTop => direction switch
    {
        EDirection.Vertical => cullingPadding.top,
        EDirection.Horizontal => cullingPadding.left,
        _ => throw new ArgumentOutOfRangeException()
    };

    private float CullingSpacing => direction switch
    {
        EDirection.Vertical => spacing.y,
        EDirection.Horizontal => spacing.x,
        _ => throw new ArgumentOutOfRangeException()
    };

    protected override void UpdateCellsPos()
    {
        for (int idx = presentMinIdx; idx < presentMaxIdx; idx++)
        {
            var cell = cells[idx];
            cell.anchorMin = new Vector2(0f, 1f);
            cell.anchorMax = new Vector2(0f, 1f);
            cell.pivot = new Vector2(0f, 1f);

            var v = GetCellPosInUGUISpace(GetRow(idx), GetCol(idx));
            cell.anchoredPosition = v;
            updateCell?.Invoke(cell, idx);
        }
    }

    private Vector2 GetCellPos(int row, int col)
    {
        float x = col * (size.x + spacing.x);
        float y = -row * (size.y + spacing.y);
        return new Vector2(x, y);
    }

    private Vector2 GetCellPosInUGUISpace(int row, int col)
    {
        return GetCellPos(row, col) - new Vector2(0, -Pos);
    }

    private int GetRow(int idx)
    {
        return Mathf.FloorToInt(idx / (float)countInLine);
    }

    private int GetCol(int idx)
    {
        return (idx % countInLine + countInLine) % countInLine;
    }

    private float LineWidth => direction switch
    {
        EDirection.Vertical => size.y + spacing.y,
        EDirection.Horizontal => size.x + spacing.x,
        _ => throw new ArgumentOutOfRangeException()
    };

    private int LineCount => Mathf.CeilToInt(totalCount / (float)countInLine);

    protected override float GetSnapPos(float pos)
    {
        return Mathf.Round(pos / LineWidth) * LineWidth;
    }

    protected override float ContentLength => isInfinite ? float.PositiveInfinity : LineWidth * LineCount;
    protected override float WindowLength
    {
        get
        {
            return direction switch
            {
                EDirection.Vertical => content.rect.height,
                EDirection.Horizontal => content.rect.width,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}