using System;
using UnityEngine;
using UnityEngine.UI;

namespace KiraraLoopScroll
{
    [AddComponentMenu("Kirara Loop Scroll/Linear Scroll View")]
    public class LinearScrollView : Scroller
    {
        // 内边距
        public Padding padding;

        // Item的间距
        public float spacing;

        public EItemAlignment itemAlignment = EItemAlignment.LeftOrUpper;

        public ScrollFunc.UpdateItem updateItem;

        private struct Item
        {
            public RectTransform rectTransform;
            public Vector2 size;

            public Item(RectTransform rectTransform, Vector2 size)
            {
                this.rectTransform = rectTransform;
                this.size = size;
            }
        }
        private readonly Deque<Item> items = new();
        private int itemFrontIndex; // 第一个Item的索引
        private int itemBackIndex; // 最后一个Item的索引(左闭右开区间)
        private float itemFrontPos; // 第一个Item的起始位置
        private float itemBackPos; // 最后一个Item的结束位置
        // (spacing只在每个Item之间，外侧没有)

        private float TopPadding => direction switch
        {
            EDirection.Horizontal => padding.left,
            EDirection.Vertical => padding.top,
            _ => throw new IndexOutOfRangeException()
        };

        private float BottomPadding => direction switch
        {
            EDirection.Horizontal => padding.right,
            EDirection.Vertical => padding.bottom,
            _ => throw new IndexOutOfRangeException()
        };

        private float LeftPadding => direction switch
        {
            EDirection.Horizontal => padding.bottom,
            EDirection.Vertical => padding.left,
            _ => throw new IndexOutOfRangeException()
        };

        private float RightPadding => direction switch
        {
            EDirection.Horizontal => padding.top,
            EDirection.Vertical => padding.right,
            _ => throw new IndexOutOfRangeException()
        };

        // protected void ScrollToEdgeWhenOutside()
        // {
        //     if (itemFrontIndex == 0)
        //     {
        //         float dist = itemFrontPos - StartPadding - Pos;
        //         if (dist > 0f)
        //         {
        //             Scroll(dist, elasticDuration);
        //             return;
        //         }
        //     }
        //     if ()
        // }

        protected override float PosToEdge
        {
            get
            {
                if (items.Count == 0) return 0f;

                if (itemFrontIndex == 0 && itemBackIndex == _totalCount &&
                    itemBackPos - itemFrontPos < DirViewportSize)
                {
                    // 所有Item都在视口内
                    return itemFrontPos - TopPadding - Pos;
                }

                if (itemFrontIndex == 0)
                {
                    float dist = itemFrontPos - TopPadding - Pos;
                    if (dist > 0f)
                    {
                        return dist;
                    }
                }

                if (itemBackIndex == _totalCount)
                {
                    float dist = itemBackPos + BottomPadding - (Pos + DirViewportSize);
                    if (dist < 0f)
                    {
                        // 说明视口尾部超出内容
                        return dist;
                    }
                }

                return 0f;
            }
        }

        protected override float ContentSize
        {
            get
            {
                if (itemFrontIndex == 0 && itemBackIndex == _totalCount)
                {
                    return itemBackPos - itemFrontPos + TopPadding + BottomPadding;
                }
                return -1;
            }
        }

        private float GetV(Vector2 v)
        {
            return direction switch
            {
                EDirection.Horizontal => v.x,
                EDirection.Vertical => v.y,
                _ => throw new IndexOutOfRangeException()
            };
        }

        private float GetH(Vector2 v)
        {
            return direction switch
            {
                EDirection.Horizontal => v.y,
                EDirection.Vertical => v.x,
                _ => throw new IndexOutOfRangeException()
            };
        }

        private float ViewportWidth => direction switch
        {
            EDirection.Horizontal => viewport.rect.height,
            EDirection.Vertical => viewport.rect.width,
            _ => throw new IndexOutOfRangeException()
        };

        protected override void UpdateItems()
        {
            // 裁剪
            const int maxIterations = 1000;
            int i = 0;

            while (items.Count > 0 && Pos > itemFrontPos + GetV(items.Front.size) && i < maxIterations)
            {
                PopFront();
                i++;
            }

            while (items.Count > 0 && Pos + DirViewportSize < itemBackPos - GetV(items.Back.size) && i < maxIterations)
            {
                PopBack();
                i++;
            }

            while (itemFrontIndex > 0 && Pos < itemFrontPos - spacing && i < maxIterations)
            {
                PushFront();
                i++;
            }

            while (itemBackIndex < _totalCount && Pos + DirViewportSize > itemBackPos + spacing && i < maxIterations)
            {
                PushBack();
                i++;
            }

            if (i == maxIterations)
            {
                Debug.LogWarning("循环滚动: UpdateItems()迭代次数过多");
            }

            // 设置位置
            float pos = itemFrontPos;
            for (i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var rectTransform = item.rectTransform;

                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);

                rectTransform.anchoredPosition = GetItemUGUIPos(item, pos);
                updateItem?.Invoke(rectTransform, itemFrontIndex + i);
                pos += GetV(item.size) + spacing;
            }
        }

        private Vector2 GetItemUGUIPos(Item item, float pos)
        {
            float y = pos - Pos;
            float x = 0f;
            if (itemAlignment == EItemAlignment.LeftOrUpper)
            {
                x = LeftPadding;
            }
            else if (itemAlignment == EItemAlignment.Center)
            {
                x = ViewportWidth * 0.5f - GetH(item.size) * 0.5f;
            }
            else if (itemAlignment == EItemAlignment.RightOrLower)
            {
                x = ViewportWidth - RightPadding - GetH(item.size);
            }

            if (direction == EDirection.Horizontal)
            {
                (x, y) = (y, x);
            }
            y = -y;
            return new Vector2(x, y);
        }

        private void PopFront()
        {
            var item = items.Front;
            returnObject(item.rectTransform.gameObject);

            items.PopFront();
            itemFrontPos += GetV(item.size);
            if (items.Count > 0)
            {
                itemFrontPos += spacing;
            }
            itemFrontIndex++;
        }

        private void PopBack()
        {
            var item = items.Back;
            returnObject(item.rectTransform.gameObject);

            items.PopBack();
            itemBackPos -= GetV(item.size);
            if (items.Count > 0)
            {
                itemBackPos -= spacing;
            }
            itemBackIndex--;
        }

        private void PushFront()
        {
            var go = getObject(itemFrontIndex - 1);
            provideData?.Invoke(go, itemFrontIndex - 1);
            go.transform.SetParent(viewport, false);

            var rectTransform = (RectTransform)go.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var size = LoopScrollSizeUtils.GetPreferredSize(rectTransform);

            items.PushFront(new Item(rectTransform, size));
            if (items.Count > 1)
            {
                itemFrontPos -= spacing;
            }
            itemFrontPos -= GetV(size);
            itemFrontIndex--;
        }

        private void PushBack()
        {
            var go = getObject(itemBackIndex);
            provideData?.Invoke(go, itemBackIndex);
            go.transform.SetParent(viewport, false);

            var rectTransform = (RectTransform)go.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var size = LoopScrollSizeUtils.GetPreferredSize(rectTransform);

            items.PushBack(new Item(rectTransform, size));
            if (items.Count > 1)
            {
                itemBackPos += spacing;
            }
            itemBackPos += GetV(size);
            itemBackIndex++;
        }

        public void RefreshToStart()
        {
            // Debug.Log("Refresh To Start");
            while (items.Count > 0)
            {
                PopBack();
            }
            itemFrontPos = 0;
            itemBackPos = 0;
            itemFrontIndex = 0;
            itemBackIndex = 0;
            SetPos(0, true);
        }

        public void RefreshToEnd()
        {
            // Debug.Log("Refresh To End");
            while (items.Count > 0)
            {
                PopFront();
            }
            itemFrontPos = DirViewportSize;
            itemBackPos = DirViewportSize;
            itemFrontIndex = _totalCount;
            itemBackIndex = _totalCount;
            SetPos(0, true);
            if (itemFrontIndex == 0 && itemFrontPos > TopPadding)
            {
                SetPos(itemFrontPos - TopPadding, true);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            itemFrontPos = TopPadding;
            itemBackPos = TopPadding;
        }
    }
}