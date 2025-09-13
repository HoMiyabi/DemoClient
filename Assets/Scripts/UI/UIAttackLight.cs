using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIAttackLight : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image leftImg;
        private UnityEngine.UI.Image rightImg;
        private UnityEngine.UI.Image upImg;
        private UnityEngine.UI.Image downImg;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b    = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            leftImg  = b.Q<UnityEngine.UI.Image>(0, "leftImg");
            rightImg = b.Q<UnityEngine.UI.Image>(1, "rightImg");
            upImg    = b.Q<UnityEngine.UI.Image>(2, "upImg");
            downImg  = b.Q<UnityEngine.UI.Image>(3, "downImg");
        }
        #endregion

        public Sprite redLightSprite;
        public Sprite yellowLightSprite;

        private Transform _wsFollow;
        private RectTransform rectTransform;

        private void Awake()
        {
            BindUI();
            rectTransform = transform as RectTransform;
        }

        public UIAttackLight Set(bool isYellow, Transform wsFollow)
        {
            _wsFollow = wsFollow;

            Sprite sprite = null;
            if (isYellow)
            {
                sprite = yellowLightSprite;
            }
            else
            {
                sprite = redLightSprite;
            }

            SetImage(leftImg, sprite);
            SetImage(rightImg, sprite);
            SetImage(upImg, sprite);
            SetImage(downImg, sprite);
            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(0.6f);
                Destroy(gameObject);
            });

            RectUtils.SetRectWorldPos(rectTransform, wsFollow.position);

            return this;
        }

        private void Update()
        {
            RectUtils.SetRectWorldPos(rectTransform, _wsFollow.position);
        }

        private void SetImage(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            image.SetNativeSize();

            image.transform.DOScaleX(2.5f, 0.3f);
            image.DOFade(0, 0.5f);
        }
    }
}