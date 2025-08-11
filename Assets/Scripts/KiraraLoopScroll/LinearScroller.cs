using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class LinearScroller : Scroller
    {
        public float spacing;

        private struct Item
        {
            public RectTransform rectTransform;
            public float size;

            public Item(RectTransform rectTransform, float size)
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

        protected override float PosToEdge
        {
            get
            {
                if (items.Count == 0) return 0f;

                if (itemFrontIndex == 0 && itemBackIndex == totalCount &&
                    itemBackPos - itemFrontPos < ViewSize)
                {
                    // 所有Item都在视口内
                    return itemFrontPos - Pos;
                }

                if (itemFrontIndex == 0)
                {
                    float dist = itemFrontPos - Pos;
                    if (dist > 0f)
                    {
                        return dist;
                    }
                }

                if (itemBackIndex == totalCount)
                {
                    float dist = itemBackPos - (Pos + ViewSize);
                    if (dist < 0f)
                    {
                        // 说明视口尾部超出内容
                        return dist;
                    }
                }

                return 0f;
            }
        }

        protected override float ContentSize => -1f;

        protected override void UpdateItems()
        {
            // 裁剪
            const int maxIterations = 1000;
            int i = 0;

            while (items.Count > 0 && Pos > itemFrontPos + items.Front.size && i < maxIterations)
            {
                PopFront();
                i++;
            }

            while (items.Count > 0 && Pos + ViewSize < itemBackPos - items.Back.size && i < maxIterations)
            {
                PopBack();
                i++;
            }

            while (itemFrontIndex > 0 && Pos < itemFrontPos - spacing && i < maxIterations)
            {
                PushFront();
                i++;
            }

            while (itemBackIndex < totalCount && Pos + ViewSize > itemBackPos + spacing && i < maxIterations)
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

                rectTransform.anchoredPosition = direction switch
                {
                    EDirection.Horizontal => new Vector2(-(pos - Pos), 0f),
                    EDirection.Vertical => new Vector2(0f, -(pos - Pos)),
                    _ => throw new IndexOutOfRangeException()
                };
                pos += item.size + spacing;
            }
        }

        private void PopFront()
        {
            var item = items.Front;
            returnObject(item.rectTransform.gameObject);

            items.PopFront();
            itemFrontPos += item.size;
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
            itemBackPos -= item.size;
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
            go.transform.SetParent(content, false);

            var rectTransform = (RectTransform)go.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            float size = LoopScrollSizeUtils.GetPreferredSize(rectTransform, (int)direction);

            items.PushFront(new Item(rectTransform, size));
            if (items.Count > 1)
            {
                itemFrontPos -= spacing;
            }
            itemFrontPos -= size;
            itemFrontIndex--;
        }

        private void PushBack()
        {
            var go = getObject(itemBackIndex);
            provideData?.Invoke(go, itemBackIndex);
            go.transform.SetParent(content, false);

            var rectTransform = (RectTransform)go.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            float size = LoopScrollSizeUtils.GetPreferredSize(rectTransform, (int)direction);

            items.PushBack(new Item(rectTransform, size));
            if (items.Count > 1)
            {
                itemBackPos += spacing;
            }
            itemBackPos += size;
            itemBackIndex++;
        }

        public void Refresh()
        {
            while (items.Count > 0)
            {
                PopBack();
            }
            UpdateItems();
        }
    }
}