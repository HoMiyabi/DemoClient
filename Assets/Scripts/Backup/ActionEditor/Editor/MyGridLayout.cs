/*using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class MyGridLayout
    {
        private EditorWindow window;
        private float width;
        private float height;
        private float hSpacing;
        private float vSpacing;

        public MyGridLayout(EditorWindow window, float width, float height, float hSpacing, float vSpacing)
        {
            this.window = window;
            this.width = width;
            this.height = height;
            this.hSpacing = hSpacing;
            this.vSpacing = vSpacing;
        }

        public Rect Rect00 => new(0, 0, width, height);

        public Rect Rect01 => new(width + hSpacing, 0, window.position.width - width - hSpacing, height);

        public Rect Rect10 => new(0, height + vSpacing, width, window.position.height - height - vSpacing);

        public Rect Rect11 => new(width + hSpacing, height + vSpacing,
            window.position.width - width - hSpacing, window.position.height - height - vSpacing);

        public Rect RectHSpacing => new(width, 0, hSpacing, window.position.height);

        public Rect RectVSpacing => new(0, height, window.position.width, vSpacing);
    }
}*/