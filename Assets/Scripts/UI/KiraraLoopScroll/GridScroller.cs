using System;
using System.Collections.Generic;
using Kirara.UI;
using UnityEngine;

public class GridScroller : Scroller
{
    public Vector2 size = new(100f, 100f);
    public Vector2 spacing = new(10f, 10f);

    // 裁切时，把它当做size + cullingPadding的大小来看
    [Header("Culling Padding Left Right Top Bottom")]
    public Vector4 cullingPadding = Vector4.zero;

    public int columnCount = 3;

    // 左闭右开
    private int _minIdx;
    private int _maxIdx;
    public readonly Dictionary<int, RectTransform> cells = new();
    private readonly Stack<RectTransform> pool = new();

    public Action<RectTransform, int> updateCell;

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
        if (currMinIdx > _minIdx)
        {
            for (int idx = _minIdx; idx < Mathf.Min(currMinIdx, _maxIdx); idx++)
            {
                ReturnObjectAt(idx);
            }
        }

        if (currMaxIdx < _maxIdx)
        {
            for (int idx = Mathf.Max(currMaxIdx, _minIdx); idx < _maxIdx; idx++)
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
        if (currMinIdx < _minIdx)
        {
            for (int idx = currMinIdx; idx < Mathf.Min(currMaxIdx, _minIdx); idx++)
            {
                GetObjectAt(idx);
            }
        }
        if (currMaxIdx > _maxIdx)
        {
            for (int idx = Mathf.Max(currMinIdx, _maxIdx); idx < currMaxIdx; idx++)
            {
                GetObjectAt(idx);
            }
        }
    }

    protected override void CullCells()
    {
        int currMinIdx = MinIdx;
        int currMaxIdx = MaxIdx;
        CheckReturnObjects(currMinIdx, currMaxIdx);
        CheckGetObjects(currMinIdx, currMaxIdx);
        foreach (var cell in pool)
        {
            cell.gameObject.SetActive(false);
        }
        _minIdx = currMinIdx;
        _maxIdx = currMaxIdx;
    }

    private int GetIdx(int row, int col)
    {
        return row * columnCount + col;
    }

    private int MinIdx
    {
        get
        {
            int minLine = Mathf.FloorToInt((Pos - CullingBottom + UsefulSpacing) / LineWidth);
            int idx = minLine * columnCount;
            return isInfinite ? idx : Mathf.Max(idx, 0);
        }
    }

    private int MaxIdx
    {
        get
        {
            int maxLine = Mathf.CeilToInt((Pos + WindowLength + CullingTop) / LineWidth);
            int idx = maxLine * columnCount;
            return isInfinite ? idx : Mathf.Min(idx, totalCount);
        }
    }

    private float CullingBottom => direction switch
    {
        EDirection.Vertical => cullingPadding.w,
        EDirection.Horizontal => cullingPadding.y,
        _ => throw new ArgumentOutOfRangeException()
    };

    private float CullingTop => direction switch
    {
        EDirection.Vertical => cullingPadding.z,
        EDirection.Horizontal => cullingPadding.x,
        _ => throw new ArgumentOutOfRangeException()
    };

    private float UsefulSpacing => direction switch
    {
        EDirection.Vertical => spacing.y,
        EDirection.Horizontal => spacing.x,
        _ => throw new ArgumentOutOfRangeException()
    };

    protected override void UpdateCellsPos()
    {
        for (int idx = _minIdx; idx < _maxIdx; idx++)
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
        return Mathf.FloorToInt(idx / (float)columnCount);
    }

    private int GetCol(int idx)
    {
        return (idx % columnCount + columnCount) % columnCount;
    }

    private float LineWidth => direction switch
    {
        EDirection.Vertical => size.y + spacing.y,
        EDirection.Horizontal => size.x + spacing.x,
        _ => throw new ArgumentOutOfRangeException()
    };

    private int LineCount => Mathf.CeilToInt(totalCount / (float)columnCount);

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