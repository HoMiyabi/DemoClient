using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISelectStickerCell : MonoBehaviour
    {
        #region View
        private Button Btn;
        private Image  Img;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Btn   = c.Q<Button>(0, "Btn");
            Img   = c.Q<Image>(1, "Img");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        public void Set(Sprite sprite, UnityAction onClick)
        {
            Img.sprite = sprite;
            Btn.onClick.AddListener(onClick);
        }
    }
}