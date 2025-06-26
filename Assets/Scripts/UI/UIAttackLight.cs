using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIAttackLight : MonoBehaviour
    {
        public Sprite redLight;
        public Sprite yellowLight;

        public Image imgLeft;
        public Image imgRight;
        public Image imgUp;
        public Image imgDown;

        private Transform wsFollow;
        private RectTransform rectTransform;

        public UIAttackLight Set(bool isYellow, Transform wsFollow)
        {
            this.wsFollow = wsFollow;

            Sprite sprite = null;
            if (isYellow)
            {
                sprite = yellowLight;
            }
            else
            {
                sprite = redLight;
            }

            SetImage(imgLeft, sprite);
            SetImage(imgRight, sprite);
            SetImage(imgUp, sprite);
            SetImage(imgDown, sprite);
            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(0.6f);
                Destroy(gameObject);
            });

            RectUtils.SetRectWorldPos(rectTransform, wsFollow.position);

            return this;
        }

        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        private void Update()
        {
            RectUtils.SetRectWorldPos(rectTransform, wsFollow.position);
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