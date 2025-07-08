using System;
using System.Collections;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIObtainNotification : MonoBehaviour
    {
        #region View
        private Image           Icon;
        private TextMeshProUGUI NameText;
        private TextMeshProUGUI CountText;
        private CanvasGroup     Group;
        private void InitUI()
        {
            var c     = GetComponent<KiraraRuntimeComponents>();
            Icon      = c.Q<Image>(0, "Icon");
            NameText  = c.Q<TextMeshProUGUI>(1, "NameText");
            CountText = c.Q<TextMeshProUGUI>(2, "CountText");
            Group     = c.Q<CanvasGroup>(3, "Group");
        }
        #endregion

        [SerializeField] private float enterDuration = 0.2f;
        [SerializeField] private float duration = 2f;
        [SerializeField] private float exitDuration = 0.2f;

        private AssetHandle handle;
        private RectTransform rect;
        private Action<UIObtainNotification> _onPlayFinished;

        private void Awake()
        {
            InitUI();

            rect = (RectTransform)transform;
        }

        private IEnumerator PlayInternal()
        {
            rect.anchoredPosition = new Vector2(-50f, rect.anchoredPosition.y);
            Group.alpha = 0.5f;
            rect.DOAnchorPosX(rect.anchoredPosition.x + 50f, enterDuration);
            yield return Group.DOFade(1f, enterDuration).WaitForCompletion();

            yield return new WaitForSeconds(duration);

            yield return Group.DOFade(0f, exitDuration).WaitForCompletion();

            _onPlayFinished?.Invoke(this);
        }

        public void Play(Action<UIObtainNotification> onPlayFinished)
        {
            _onPlayFinished = onPlayFinished;
            StartCoroutine(PlayInternal());
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
        }

        public void Set(string iconLoc, string name_, int count)
        {
            Clear();
            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(iconLoc);
            Icon.sprite = handle.AssetObject as Sprite;
            NameText.text = name_;
            CountText.text = count.ToString();
        }
    }
}