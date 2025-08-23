using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace KiraraLoopScroll
{
    [AddComponentMenu("Kirara Loop Scroll/Grid Scroll View")]
    public class GridScrollView : Scroller
    {
        // 不可见阈值
        [FormerlySerializedAs("invisibleThreshold")] public int loadThreshold = 0;

        // 内边距
        public Padding padding;

        // Item的大小
        [FormerlySerializedAs("size")] public Vector2 itemSize = new(100f, 100f);

        // Item的间距
        public Vector2 spacing = new(10f, 10f);

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

        private int CountInLine
        {
            get
            {
                if (!flexibleCountInLine)
                {
                    return countInLine;
                }
                float sz = SubDirViewSize;
                sz -= SubDirPadding;
                int ans = Mathf.FloorToInt((sz + SubDirSpacing + 1f) / (SubDirItemSize + SubDirSpacing));
                return Mathf.Max(1, ans);
            }
        }

        private float SubDirViewSize => direction switch
        {
            EDirection.Horizontal => content.rect.height,
            EDirection.Vertical => content.rect.width,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float SubDirPadding => direction switch
        {
            EDirection.Horizontal => padding.top + padding.bottom,
            EDirection.Vertical => padding.left + padding.right,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float SubDirItemSize => direction switch
        {
            EDirection.Horizontal => itemSize.y,
            EDirection.Vertical => itemSize.x,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float SubDirSpacing => direction switch
        {
            EDirection.Horizontal => spacing.y,
            EDirection.Vertical => spacing.x,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float StartPadding => direction switch
        {
            EDirection.Horizontal => padding.left,
            EDirection.Vertical => padding.top,
            _ => throw new ArgumentOutOfRangeException()
        };

        private float EndPadding => direction switch
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
                int minLine = Mathf.FloorToInt((Pos - StartPadding + LineSpacing) / (LineWidth + LineSpacing));
                minLine -= loadThreshold;
                int index = minLine * CountInLine;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        // 加载最大Index(不包含)
        private int ShouldLoadBackIndex
        {
            get
            {
                int maxLine = Mathf.CeilToInt((Pos + ViewSize - StartPadding) / (LineWidth + LineSpacing));
                maxLine += loadThreshold;
                int index = maxLine * CountInLine;
                return isInfinite ? index : Mathf.Clamp(index, 0, _totalCount);
            }
        }

        private float LineSpacing => direction switch
        {
            EDirection.Horizontal => spacing.x,
            EDirection.Vertical => spacing.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private Vector2 GetItemPos(int lineNum, int subNum)
        {
            int row = lineNum;
            int col = subNum;
            if (direction == EDirection.Horizontal)
            {
                (row, col) = (col, row);
            }
            var delta = itemSize + spacing;
            float x = col * delta.x;
            float y = row * delta.y;
            return new Vector2(x, y);
        }

        private Vector2 DirectionPos => direction switch
        {
            EDirection.Horizontal => new Vector2(Pos, 0),
            EDirection.Vertical => new Vector2(0, Pos),
            _ => throw new ArgumentOutOfRangeException()
        };

        private Vector2 GetItemPosInUGUISpace(int lineNum, int subNum)
        {
            var pos = GetItemPos(lineNum, subNum) + new Vector2(padding.left, padding.top) - DirectionPos;
            pos.y = -pos.y;
            return pos;
        }

        private int GetLineNum(int index)
        {
            return Mathf.FloorToInt(index / (float)CountInLine);
        }

        private int GetSubNum(int index)
        {
            return (index % CountInLine + CountInLine) % CountInLine;
        }

        private Vector2 GetItemPosInUGUISpace(int index)
        {
            int lineNum = GetLineNum(index);
            int subNum = GetSubNum(index);
            return GetItemPosInUGUISpace(lineNum, subNum);
        }

        // 排宽
        private float LineWidth => direction switch
        {
            EDirection.Horizontal => itemSize.x,
            EDirection.Vertical => itemSize.y,
            _ => throw new ArgumentOutOfRangeException()
        };

        private int LineCount => Mathf.CeilToInt(_totalCount / (float)CountInLine);

        protected override float GetSnapPos(float pos)
        {
            float dirNormalizedPos = (pos - StartPadding + LineSpacing) / (LineWidth + LineSpacing);
            return Mathf.Round(dirNormalizedPos) * (LineWidth + LineSpacing) + StartPadding;
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
                if (lineCount >= 2)
                {
                    ans += LineSpacing * (lineCount - 1);
                }
                ans += StartPadding + EndPadding;
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

                item.anchoredPosition = GetItemPosInUGUISpace(index);
                updateItem?.Invoke(item, index);
            }
        }
    }
}