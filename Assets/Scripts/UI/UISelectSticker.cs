using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kirara.Service;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using UnityEngine.Pool;
using YooAsset;

namespace Kirara.UI
{
    public class UISelectSticker : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private KiraraLoopScroll.GridScrollView SelectStickerLoopScroll;
        private UnityEngine.CanvasGroup         CanvasGroup;
        private UnityEngine.RectTransform       Box;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b                   = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SelectStickerLoopScroll = b.Q<KiraraLoopScroll.GridScrollView>(0, "SelectStickerLoopScroll");
            CanvasGroup             = b.Q<UnityEngine.CanvasGroup>(1, "CanvasGroup");
            Box                     = b.Q<UnityEngine.RectTransform>(2, "Box");
        }
        #endregion

        public GameObject SelectStickerItemPrefab;

        private ChatPanel chatPanel;
        private List<int> stickerConfigIds;
        private List<Sprite> stickerSprites;

        private void Awake()
        {
            BindUI();
            stickerConfigIds = ListPool<int>.Get();
            stickerSprites = ListPool<Sprite>.Get();

            foreach (var item in ConfigMgr.tb.TbIconSticker.DataList)
            {
                stickerConfigIds.Add(item.Id);

                var handle = YooAssets.LoadAssetSync<Sprite>(item.Location);
                stickerSprites.Add(handle.AssetObject as Sprite);
            }

            SelectStickerLoopScroll.SetSource(new LoopScrollGOPool(SelectStickerItemPrefab, transform));
            SelectStickerLoopScroll.provideData = ProvideData;
            SelectStickerLoopScroll._totalCount = stickerConfigIds.Count;
        }

        public void Set(ChatPanel chatPanel)
        {
            this.chatPanel = chatPanel;
        }

        private void OnDestroy()
        {
            ListPool<int>.Release(stickerConfigIds);
            ListPool<Sprite>.Release(stickerSprites);
        }

        private void ProvideData(GameObject go, int index)
        {
            var cell = go.GetComponent<UISelectStickerItem>();
            cell.Set(stickerSprites[index], () => Cell_onClick(index));
        }

        private void Cell_onClick(int idx)
        {
            Hide();
            SocialService.SendSticker(chatPanel.ChattingPlayer, stickerConfigIds[idx]).Forget();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            CanvasGroup.DOKill();
            Box.DOKill();

            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f);

            Box.anchoredPosition = new Vector2(0, -Box.rect.height * 0.05f);
            Box.DOAnchorPos(Vector2.zero, 0.1f);
        }

        public void Hide()
        {
            CanvasGroup.DOKill();
            Box.DOKill();

            CanvasGroup.DOFade(0f, 0.1f);
            Box.DOAnchorPos(new Vector2(0, -Box.rect.height * 0.05f), 0.1f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}