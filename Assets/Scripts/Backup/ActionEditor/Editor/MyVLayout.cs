/*using UnityEngine;

namespace Kirara.ActionEditor
{
    public class MyVLayout
    {
        private float width;
        private float height;
        private float cellHeight;
        private float spacing;

        public MyVLayout(float width, float height, float cellHeight, float spacing)
        {
            this.width = width;
            this.height = height;
            this.cellHeight = cellHeight;
            this.spacing = spacing;
        }

        public Rect Rect(int index)
        {
            return new Rect(0, index * (cellHeight + spacing), width, cellHeight);
        }

        public bool TryRect(int index, out Rect rect)
        {
            rect = Rect(index);
            return rect.yMin < height && rect.yMax > 0;
        }

        public Rect Spacing(int index)
        {
            return new Rect(0, index * (cellHeight + spacing) + cellHeight, width, spacing);
        }
    }
}*/