using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIInventoryRankBar : MonoBehaviour
    {
        #region View
        private Image Img;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Img   = c.Q<Image>(0, "Img");
        }
        #endregion

        [SerializeField] private Color SColor = Color.white;
        [SerializeField] private Color AColor = Color.white;
        [SerializeField] private Color BColor = Color.white;
        [SerializeField] private Color CColor = Color.white;

        private void Awake()
        {
            InitUI();
        }

        public void Set(string rank)
        {
            if (rank == "S")
            {
                Img.color = SColor;
            }
            else if (rank == "A")
            {
                Img.color = AColor;
            }
            else if (rank == "B")
            {
                Img.color = BColor;
            }
            else if (rank == "C")
            {
                Img.color = CColor;
            }
            else
            {
                Img.color = Color.clear;
                Debug.LogWarning($"{rank} 不是合法的Rank");
            }
        }
    }
}