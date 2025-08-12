using UnityEngine;
using UnityEngine.Events;

namespace Kirara.UI
{
    public class UISelectStickerItem : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button Btn;
        private UnityEngine.UI.Image  Img;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Btn   = c.Q<UnityEngine.UI.Button>(0, "Btn");
            Img   = c.Q<UnityEngine.UI.Image>(1, "Img");
        }
        #endregion

        public void Set(Sprite sprite, UnityAction onClick)
        {
            BindUI();

            Img.sprite = sprite;
            Btn.onClick.AddListener(onClick);
        }
    }
}