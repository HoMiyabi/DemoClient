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
        private int itemEndIndex; // 最后一个Item的索引(左闭右闭区间)
        private float itemStartPos; // 第一个Item的起始位置
        private float itemEndPos; // 最后一个Item的结束位置(外侧没有spacing)

        protected override bool CurrentPosOutOfContent { get; } = false;
        protected override float ContentSize { get; } = 1000f;
        protected override void UpdateItems()
        {
            // 裁剪
            const int maxIterations = 1000;
            int i = 0;

            while (Pos > itemStartPos && items.Count > 0 && i < maxIterations)
            {
                var item = items.Front;
                if (Pos > itemStartPos + item.size)
                {
                    returnObject(item.rectTransform.gameObject);

                    items.PopFront();
                    itemStartPos += item.size + spacing;
                    itemStartIndex++;
                }
                else
                {
                    break;
                }

                i++;
            }

            while (Pos + ViewSize < itemEndPos && items.Count > 0 && i < maxIterations)
            {
                var item = items.Back;
                if (Pos + ViewSize < itemEndPos - item.size)
                {
                    returnObject(item.rectTransform.gameObject);

                    items.PopBack();
                    itemEndPos -= item.size + spacing;
                    itemEndIndex--;
                }
                else
                {
                    break;
                }

                i++;
            }

            while (Pos < itemStartPos - spacing && itemStartIndex > 0 && i < maxIterations)
            {
                var go = getObject(itemStartIndex - 1);
                provideData?.Invoke(go, itemStartIndex - 1);
                go.transform.SetParent(content, false);

                var rectTransform = (RectTransform)go.transform;
                float size = LayoutUtility.GetPreferredSize(rectTransform, (int)direction);

                items.PushFront(new Item(rectTransform, size));
                itemStartPos -= size + spacing;
                itemStartIndex--;

                i++;
            }

            while (Pos + ViewSize > itemEndPos + spacing && itemEndIndex < totalCount - 1 && i < maxIterations)
            {
                var go = getObject(itemEndIndex + 1);
                provideData?.Invoke(go, itemEndIndex + 1);
                go.transform.SetParent(content, false);

                var rectTransform = (RectTransform)go.transform;
                float size = LayoutUtility.GetPreferredSize(rectTransform, (int)direction);

                items.PushBack(new Item(rectTransform, size));
                itemEndPos += size + spacing;
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
                    _ => throw new System.NotImplementedException(),
                };
                pos += item.size + spacing;
            }
        }
    }
}