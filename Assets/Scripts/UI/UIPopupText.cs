using System.Collections;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace Kirara.UI
{
    public class UIPopupText : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI   Text;
        private UnityEngine.CanvasGroup CanvasGroup;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text        = b.Q<TMPro.TextMeshProUGUI>(0, "Text");
            CanvasGroup = b.Q<UnityEngine.CanvasGroup>(1, "CanvasGroup");
        }
        #endregion

        public float scaleUpDuration = 0.2f;
        public float scaleDownDuration = 0.5f;
        public float idleDuration = 0.1f;
        public float fadeOutDuration = 0.2f;

        public float scaleUpValue = 1.2f;
        public float scaleDownValue = 0.6f;

        private Transform _follow;
        private Vector3 _localPos;
        private RectTransform _rectTransform;
        private StringBuilder _sb = new();

        private void Awake()
        {
            BindUI();

            _rectTransform = (RectTransform)transform;
        }

        public UIPopupText SetInfo(Transform follow, Vector3 localPos, double dmg, bool isCrit)
        {
            _follow = follow;
            _localPos = localPos;
            UpdatePos();

            _sb.AppendFormat("{0:F0}", dmg);
            if (isCrit)
            {
                _sb.Append("!!");
            }
            Text.SetText(_sb);

            return this;
        }

        private IEnumerator PlayInternal()
        {
            // 放大一点 缩小很多 渐隐上移
            yield return transform.DOScale(scaleUpValue, scaleUpDuration).WaitForCompletion();
            yield return transform.DOScale(scaleDownValue, scaleDownDuration).WaitForCompletion();
            yield return new WaitForSeconds(idleDuration);
            yield return CanvasGroup.DOFade(0f, fadeOutDuration).WaitForCompletion();
            Destroy(gameObject);
        }

        public UIPopupText Play()
        {
            StartCoroutine(PlayInternal());
            return this;
        }

        private void UpdatePos()
        {
            RectUtils.SetRectWorldPos(_rectTransform, _follow, _localPos, false);
        }

        private void Update()
        {
            UpdatePos();
        }
    }
}