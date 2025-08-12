using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIItemStar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image StarImg;
        private UnityEngine.UI.Image StarImg1;
        private UnityEngine.UI.Image StarImg2;
        private UnityEngine.UI.Image StarImg3;
        private UnityEngine.UI.Image StarImg4;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c    = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            StarImg  = c.Q<UnityEngine.UI.Image>(0, "StarImg");
            StarImg1 = c.Q<UnityEngine.UI.Image>(1, "StarImg1");
            StarImg2 = c.Q<UnityEngine.UI.Image>(2, "StarImg2");
            StarImg3 = c.Q<UnityEngine.UI.Image>(3, "StarImg3");
            StarImg4 = c.Q<UnityEngine.UI.Image>(4, "StarImg4");
        }
        #endregion

        [SerializeField]
        private Sprite whiteStar;
        [SerializeField]
        private Sprite blackStar;

        private Image[] imgs;

        public void SetStar(int num)
        {
            BindUI();
            if (imgs == null)
            {
                imgs = new[] { StarImg, StarImg1, StarImg2, StarImg3, StarImg4 };
            }

            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].sprite = i < num ? whiteStar : blackStar;
            }
        }
    }
}