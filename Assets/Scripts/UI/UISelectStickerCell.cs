using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISelectStickerCell : MonoBehaviour
    {
        private Button Btn;
        private Image Img;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Btn = c.Q<Button>("Btn");
            Img = c.Q<Image>("Img");
        }

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