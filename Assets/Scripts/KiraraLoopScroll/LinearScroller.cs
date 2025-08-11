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
        private int itemStartIndex; // 第一个Item的索引
        private int itemEndIndex; // 最后一个Item的索引(左闭右开区间)
        private float itemStartPos; // 第一个Item的起始位置
        private float itemEndPos; // 最后一个Item的结束位置
        // (spacing只在每个Item之间，外侧没有)

        protected override float PosToEdge
        {
            get
            {
                if (items.Count == 0) return 0f;

                if (itemStartIndex == 0)
                {
                    float dist = itemStartPos - Pos;
                    if (dist > 0f)
                    {
                        return dist;
                    }
                }

                if (itemEndIndex == totalCount)
                {
                    float dist = itemEndPos - (Pos + ViewSize);
                    if (dist < 0f)
                    {
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

            while (items.Count > 0 && Pos > itemStartPos + items.Front.size && i < maxIterations)
            {
                var item = items.Front;
                returnObject(item.rectTransform.gameObject);

                items.PopFront();
                itemStartPos += item.size;
                if (items.Count > 0)
                {
                    itemStartPos += spacing;
                }
                itemStartIndex++;

                i++;
            }

            while (items.Count > 0 && Pos + ViewSize < itemEndPos - items.Back.size && i < maxIterations)
            {
                var item = items.Back;
                returnObject(item.rectTransform.gameObject);

                items.PopBack();
                itemEndPos -= item.size;
                if (items.Count > 0)
                {
                    itemEndPos -= spacing;
                }
                itemEndIndex--;

                i++;
            }

            while (itemStartIndex > 0 && Pos < itemStartPos - spacing && i < maxIterations)
            {
                var go = getObject(itemStartIndex - 1);
                provideData?.Invoke(go, itemStartIndex - 1);
                go.transform.SetParent(content, false);

                var rectTransform = (RectTransform)go.transform;
                float size = LayoutUtility.GetPreferredSize(rectTransform, (int)direction);

                items.PushFront(new Item(rectTransform, size));
                if (items.Count > 1)
                {
                    itemStartPos -= spacing;
                }
                itemStartPos -= size;
                itemStartIndex--;

                i++;
            }

            while (itemEndIndex < totalCount && Pos + ViewSize > itemEndPos + spacing && i < maxIterations)
            {
                var go = getObject(itemEndIndex);
                provideData?.Invoke(go, itemEndIndex);
                go.transform.SetParent(content, false);

                var rectTransform = (RectTransform)go.transform;
                float size = LayoutUtility.GetPreferredSize(rectTransform, (int)direction);

                items.PushBack(new Item(rectTransform, size));
                if (items.Count > 1)
                {
                    itemEndPos += spacing;
                }
                itemEndPos += size;
                itemEndIndex++;

                i++;
            }

            if (i == maxIterations)
            {
                Debug.LogWarning("循环滚动: UpdateItems()迭代次数过多");
            }

            // 设置位置
            float pos = itemStartPos;
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
    }
}