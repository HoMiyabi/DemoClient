using DG.Tweening;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class BasePanel : AbstractBasePanel
    {
        public virtual void BindUI()
        {
        }

        protected virtual void Awake()
        {
            BindUI();
        }

        protected virtual void PanelPlayEnter(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        protected virtual void PanelPlayExit(CanvasGroup canvasGroup)
        {
            canvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
        }

        protected virtual void DialogPlayEnter(CanvasGroup canvasGroup, RectTransform box)
        {
            var seq = DOTween.Sequence();
            box.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var t1 = box.DOScale(1f, 0.1f);
            seq.Join(t1);

            canvasGroup.alpha = 0f;
            var t2 = canvasGroup.DOFade(1f, 0.1f);
            seq.Join(t2);

            seq.onComplete = base.PlayEnter;
        }

        protected virtual void DialogPlayExit(CanvasGroup canvasGroup, RectTransform box)
        {
            var seq = DOTween.Sequence();
            var t1 = box.DOScale(0.8f, 0.1f);
            seq.Join(t1);

            var t2 = canvasGroup.DOFade(0f, 0.1f);
            seq.Join(t2);

            seq.onComplete = base.PlayExit;
        }
    }
}