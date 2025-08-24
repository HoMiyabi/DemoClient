using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace KiraraLoopScroll
{
    [AddComponentMenu("Kirara Loop Scroll/Grid Scroll View")]
    public class GridScrollView : Scroller
    {
        // 多加载阈值，排数
        [FormerlySerializedAs("invisibleThreshold")] public int loadThreshold = 0;

        // 内边距
        public Padding padding;

        // Item的大小
        [FormerlySerializedAs("size")] public Vector2 itemSize = new(100f, 100f);

        // Item的间距
        public Vector2 spacing = new(10f, 10f);

        public EItemAlignment itemAlignment = EItemAlignment.LeftOrUpper;

        public bool flexibleCountInLine = false;
        // 每一排的Item数量
        public int countInLine = 3;

        private readonly Deque<RectTransform> items = new();
        private int itemFrontIndex;
        private int itemBackIndex; // 左闭右开

        public ScrollFunc.UpdateItem updateItem;

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

        private int ColCount
        {
            get
            {
                if (!flexibleCountInLine)
                {
                    return countInLine;
                }
                float sz = ViewportWidth;
                sz -= LeftPadding + RightPadding;
                int ans = Mathf.FloorToInt((sz + HSpacing + 1f) / (ItemWidth + HSpacing));
                return Mathf.Max(1, ans);
            }
        }

        private float ViewportWidth => direction switch
        {
            EDirection.Horizontal => viewport.rect.height,
            EDirection.Vertical => viewport.rect.width,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float ItemWidth => direction switch
        {
            EDirection.Horizontal => itemSize.y,
            EDirection.Vertical => itemSize.x,
            _ => throw new ArgumentOutOfRangeException()
        };

        // 排宽
        private float ItemHeight => direction switch
        {
            EDirection.Horizontal => itemSize.x,
            EDirection.Vertical => itemSize.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float HSpacing => direction switch
        {
            EDirection.Horizontal => spacing.y,
            EDirection.Vertical => spacing.x,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float VSpacing => direction switch
        {
            EDirection.Horizontal => spacing.x,
            EDirection.Vertical => spacing.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float LeftPadding => direction switch
        {
            EDirection.Horizontal => padding.bottom,
            EDirection.Vertical => padding.left,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float RightPadding => direction switch
        {
            EDirection.Horizontal => padding.top,
            EDirection.Vertical => padding.right,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float TopPadding => direction switch
        {
            EDirection.Horizontal => padding.left,
            EDirection.Vertical => padding.top,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float BottomPadding => direction switch
        {
            EDirection.Horizontal => padding.right,
            EDirection.Vertical => padding.bottom,
            _ => throw new ArgumentOutOfRangeException()
        };

        // 加载最小Index
        private int ShouldLoadFrontIndex
        {
            get
            {
                int minLine = Mathf.FloorToInt((Pos - TopPadding + VSpacing) / (ItemHeight + VSpacing));
                minLine -= loadThreshold;
                int index = minLine * ColCount;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        // 加载最大Index(不包含)
        private int ShouldLoadBackIndex
        {
            get
            {
                int maxLine = Mathf.CeilToInt((Pos + ViewSize - TopPadding) / (ItemHeight + VSpacing));
                maxLine += loadThreshold;
                int index = maxLine * ColCount;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        private Vector2 GetItemUGUIPos(int rowNum, int colNum)
        {
            float y = TopPadding + rowNum * (ItemHeight + VSpacing) - Pos;

            float x = 0f;
            if (itemAlignment == EItemAlignment.LeftOrUpper)
            {
                x = LeftPadding;
            }
            else if (itemAlignment == EItemAlignment.Center)
            {
                x = ViewportWidth * 0.5f - ContentWidthWithoutPadding * 0.5f;
            }
            else if (itemAlignment == EItemAlignment.RightOrLower)
            {
                x = ViewportWidth - RightPadding - ContentWidthWithoutPadding;
            }
            x += colNum * (ItemWidth + HSpacing);

            if (direction == EDirection.Horizontal)
            {
                (x, y) = (y, x);
            }
            y = -y;
            return new Vector2(x, y);
        }

        private int GetRowNum(int index)
        {
            // 无论index为正负，都要向下取整
            return Mathf.FloorToInt(index / (float)ColCount);
        }

        private int GetColNum(int index)
        {
            return (index % ColCount + ColCount) % ColCount;
        }

        private Vector2 GetItemUGUIPos(int index)
        {
            int rowNum = GetRowNum(index);
            int colNum = GetColNum(index);
            return GetItemUGUIPos(rowNum, colNum);
        }

        private int RowCount => Mathf.CeilToInt(_totalCount / (float)ColCount);

        protected override float GetSnapPos(float pos)
        {
            float dirNormalizedPos = (pos - TopPadding + VSpacing) / (ItemHeight + VSpacing);
            return Mathf.Round(dirNormalizedPos) * (ItemHeight + VSpacing) + TopPadding;
        }

        protected override float PosToEdge
        {
            get
            {
                if (isInfinite) return 0f;

                float validMaxPos = Mathf.Max(0f, ContentSize - ViewSize);
                if (Pos < 0f)
                {
                    return -Pos; // 正数
                }
                if (Pos > validMaxPos)
                {
                    return validMaxPos - Pos; // 负数
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
            go.transform.SetParent(viewport, false);

            items.PushFront((RectTransform)go.transform);
            itemFrontIndex--;
        }

        private void PushBack()
        {
            var go = getObject(itemBackIndex);
            provideData?.Invoke(go, itemBackIndex);
            go.transform.SetParent(viewport, false);

            items.PushBack((RectTransform)go.transform);
            itemBackIndex++;
        }

        private float ContentWidthWithoutPadding
        {
            get
            {
                int colCount = ColCount;
                float ans = ItemWidth * colCount;
                if (colCount >= 2)
                {
                    ans += HSpacing * (colCount - 1);
                }
                return ans;
            }
        }


        protected override float ContentSize
        {
            get
            {
                if (isInfinite) return float.PositiveInfinity;

                float rowCount = RowCount;
                float ans = ItemHeight * rowCount;
                if (rowCount >= 2)
                {
                    ans += VSpacing * (rowCount - 1);
                }
                ans += TopPadding + BottomPadding;
                return ans;
            }
        }

        protected override void UpdateItems()
        {
            // 裁剪
            const int maxIterations = 1000;
            int i = 0;

            int shouldLoadFrontIndex = ShouldLoadFrontIndex;
            int shouldLoadBackIndex = ShouldLoadBackIndex;
            while (items.Count > 0 && itemFrontIndex < shouldLoadFrontIndex && i < maxIterations)
            {
                PopFront();
                i++;
            }
            while (items.Count > 0 && itemBackIndex > shouldLoadBackIndex && i < maxIterations)
            {
                PopBack();
                i++;
            }
            while (itemFrontIndex > shouldLoadFrontIndex && i < maxIterations)
            {
                PushFront();
                i++;
            }
            while (itemBackIndex < shouldLoadBackIndex && i < maxIterations)
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

                item.anchoredPosition = GetItemUGUIPos(index);
                updateItem?.Invoke(item, index);
            }
        }
    }
}