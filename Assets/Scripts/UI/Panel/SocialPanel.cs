using System;
using DG.Tweening;

namespace Kirara.UI.Panel
{
    public class SocialPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button   UIBackBtn;
        private UnityEngine.CanvasGroup CanvasGroup;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn   = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            CanvasGroup = b.Q<UnityEngine.CanvasGroup>(1, "CanvasGroup");
        }
        #endregion

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public override void PlayEnter()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        public override void PlayExit()
        {
            CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
        }
    }
}