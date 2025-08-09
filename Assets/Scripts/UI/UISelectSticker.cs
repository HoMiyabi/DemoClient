using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Service;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UISelectSticker : MonoBehaviour, LoopScrollDataSource
    {
        #region View
        private LoopVerticalScrollRect       SelectStickerLoopScroll;
        private SimpleLoopScrollPrefabSource SelectStickerLoopScrollPrefabSource;
        private void InitUI()
        {
            var c                               = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SelectStickerLoopScroll             = c.Q<LoopVerticalScrollRect>(0, "SelectStickerLoopScroll");
            SelectStickerLoopScrollPrefabSource = c.Q<SimpleLoopScrollPrefabSource>(1, "SelectStickerLoopScrollPrefabSource");
        }
        #endregion

        private ChatPanel chatPanel;
        private List<int> stickerConfigIds;
        private List<AssetHandle> stickerHandles;

        private void Awake()
        {
            InitUI();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < stickerHandles.Count; i++)
            {
                stickerHandles[i].Release();
                stickerHandles[i] = null;
            }
        }

        public void Set(ChatPanel chatPanel)
        {
            this.chatPanel = chatPanel;
        }

        private void Start()
        {
            stickerConfigIds = new List<int>(ConfigMgr.tb.TbIconSticker.DataList.Count);
            stickerHandles = new List<AssetHandle>(ConfigMgr.tb.TbIconSticker.DataList.Count);

            foreach (var item in ConfigMgr.tb.TbIconSticker.DataList)
            {
                stickerConfigIds.Add(item.Id);

                var handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(item.Location);
                stickerHandles.Add(handle);
            }

            SelectStickerLoopScroll.prefabSource = SelectStickerLoopScrollPrefabSource;
            SelectStickerLoopScroll.dataSource = this;
            SelectStickerLoopScroll.totalCount = stickerHandles.Count;
            SelectStickerLoopScroll.RefillCells();
        }

        public void ProvideData(Transform tra, int idx)
        {
            var cell = tra.GetComponent<UISelectStickerCell>();
            cell.Set(stickerHandles[idx].AssetObject as Sprite, () => Cell_onClick(idx));
        }

        private void Cell_onClick(int idx)
        {
            gameObject.SetActive(false);
            SocialService.SendSticker(chatPanel.ChattingPlayer, stickerConfigIds[idx]).Forget();
        }
    }
}