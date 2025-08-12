using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Service;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using UnityEngine.Pool;

namespace Kirara.UI
{
    public class UISelectSticker : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private KiraraLoopScroll.GridScrollView SelectStickerLoopScroll;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                   = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SelectStickerLoopScroll = c.Q<KiraraLoopScroll.GridScrollView>(0, "SelectStickerLoopScroll");
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
            stickerConfigIds.Clear();
            stickerSprites = ListPool<Sprite>.Get();
            stickerSprites.Clear();

            foreach (var item in ConfigMgr.tb.TbIconSticker.DataList)
            {
                stickerConfigIds.Add(item.Id);

                var handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(item.Location);
                stickerSprites.Add(handle.AssetObject as Sprite);
            }

            SelectStickerLoopScroll.SetGOSource(new LoopScrollGOPool(SelectStickerItemPrefab, transform));
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

        public void ProvideData(GameObject go, int index)
        {
            var cell = go.GetComponent<UISelectStickerItem>();
            cell.Set(stickerSprites[index], () => Cell_onClick(index));
        }

        private void Cell_onClick(int idx)
        {
            gameObject.SetActive(false);
            SocialService.SendSticker(chatPanel.ChattingPlayer, stickerConfigIds[idx]).Forget();
        }
    }
}