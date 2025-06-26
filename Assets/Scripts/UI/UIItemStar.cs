using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIItemStar : MonoBehaviour
    {
        [SerializeField]
        private Sprite whiteStar;
        [SerializeField]
        private Sprite blackStar;

        private Image[] imgs;

        private void Awake()
        {
            imgs = GetComponentsInChildren<Image>();
        }

        public void SetStar(int num)
        {
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].sprite = i < num ? whiteStar : blackStar;
            }
        }
    }
}