using System;
using UnityEngine;

namespace KiraraLoopScroll
{
    [AddComponentMenu("Kirara Loop Scroll/Grid Scroll View")]
    public class GridScrollView : Scroller
    {
        public Padding padding;
        public Vector2 size = new(100f, 100f);
        public Vector2 spacing = new(10f, 10f);

        public int countInLine = 3;

        private readonly Deque<RectTransform> items = new();
        private int itemFrontIndex;
        private int itemBackIndex; // 左闭右开

        public Action<RectTransform, int> updateCell;

        public void RefreshToStart()
        {
            while (items.Count > 0)
            {
                PopBack();
            }
            itemFrontIndex = 0;
            itemBackIndex = 0;
            UpdateItems();
        }

        // 可见最小Idx
        private int ViewFrontIndex
        {
            get
            {
                int minLine = Mathf.FloorToInt((Pos - padding.top + CullingSpacing) / (LineWidth + CullingSpacing));
                int index = minLine * countInLine;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        // 可见最大Idx
        private int ViewBackIndex
        {
            get
            {
                int maxLine = Mathf.CeilToInt((Pos + ViewSize - padding.top) / (LineWidth + CullingSpacing));
                int index = maxLine * countInLine;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        private float CullingSpacing => direction switch
        {
            EDirection.Horizontal => spacing.x,
            EDirection.Vertical => spacing.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private Vector2 GetCellPos(int row, int col)
        {
            float x = col * (size.x + spacing.x);
            float y = -row * (size.y + spacing.y);
            return new Vector2(x, y);
        }

        private Vector2 GetCellPosInUGUISpace(int row, int col)
        {
            return GetCellPos(row, col) - new Vector2(0, -Pos) + new Vector2(padding.left, -padding.top);
        }

        private int GetRow(int index)
        {
            return Mathf.FloorToInt(index / (float)countInLine);
        }

        private int GetCol(int index)
        {
            return (index % countInLine + countInLine) % countInLine;
        }

        private float LineWidth => direction switch
        {
            EDirection.Horizontal => size.x,
            EDirection.Vertical => size.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private int LineCount => Mathf.CeilToInt(_totalCount / (float)countInLine);

        protected override float GetSnapPos(float pos)
        {
            return Mathf.Round(pos / LineWidth) * LineWidth;
        }

        protected override float PosToEdge
        {
            get
            {
                if (isInfinite) return 0f;

                if (Pos < 0f)
                {
                    return -Pos;
                }
                if (ContentSize - (Pos + ViewSize) < 0)
                {
                    return ContentSize - (Pos + ViewSize);
                }
                return 0f;
            }
        }

        private void PopFront()
        {
            var item = items.Front;
            returnObject(item.gameObject);

            items.PopFront();
            itemFrontIndex++;
        }

        private void PopBack()
        {
            var item = items.Back;
            returnObject(item.gameObject);

            items.PopBack();
            itemBackIndex--;
        }

        private void PushFront()
        {
            var go = getObject(itemFrontIndex - 1);
            provideData?.Invoke(go, itemFrontIndex - 1);
            go.transform.SetParent(content, false);

            items.PushFront((RectTransform)go.transform);
            itemFrontIndex--;
        }

        private void PushBack()
        {
            var go = getObject(itemBackIndex);
            provideData?.Invoke(go, itemBackIndex);
            go.transform.SetParent(content, false);

            items.PushBack((RectTransform)go.transform);
            itemBackIndex++;
        }


        protected override float ContentSize
        {
            get
            {
                if (isInfinite) return float.PositiveInfinity;

                float lineCount = LineCount;
                float ans = LineWidth * lineCount;
                if (lineCount > 0)
                {
                    ans += CullingSpacing * (lineCount - 1);
                }
                ans += padding.top + padding.bottom;
                return ans;
            }
        }

        protected override void UpdateItems()
        {
            // 裁剪
            const int maxIterations = 1000;
            int i = 0;

            int viewFrontIndex = ViewFrontIndex;
            int viewBackIndex = ViewBackIndex;
            while (items.Count > 0 && itemFrontIndex < viewFrontIndex && i < maxIterations)
            {
                PopFront();
                i++;
            }
            while (items.Count > 0 && itemBackIndex > viewBackIndex && i < maxIterations)
            {
                PopBack();
                i++;
            }
            while (itemFrontIndex > viewFrontIndex && i < maxIterations)
            {
                PushFront();
                i++;
            }
            while (itemBackIndex < viewBackIndex && i < maxIterations)
            {
                PushBack();
                i++;
            }
            if (i == maxIterations)
            {
                Debug.LogWarning("循环滚动: UpdateItems()迭代次数过多");
            }

            // 设置位置
            for (i = 0; i < items.Count; i++)
            {
                int index = itemFrontIndex + i;
                var item = items[i];
                item.anchorMin = new Vector2(0f, 1f);
                item.anchorMax = new Vector2(0f, 1f);
                item.pivot = new Vector2(0f, 1f);

                var v = GetCellPosInUGUISpace(GetRow(index), GetCol(index));
                item.anchoredPosition = v;
                updateCell?.Invoke(item, index);
            }
        }
    }
}