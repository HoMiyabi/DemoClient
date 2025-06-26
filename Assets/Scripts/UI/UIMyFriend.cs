using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIMyFriend : MonoBehaviour, LoopScrollDataSource
    {
        private TextMeshProUGUI FriendCountText;
        private LoopVerticalScrollRect LoopScroll;
        private SimpleLoopScrollPrefabSource LoopScrollPrefabSource;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            FriendCountText = c.Q<TextMeshProUGUI>("FriendCountText");
            LoopScroll = c.Q<LoopVerticalScrollRect>("LoopScroll");
            LoopScrollPrefabSource = c.Q<SimpleLoopScrollPrefabSource>("LoopScrollPrefabSource");
        }

        private List<NOtherPlayerInfo> infos;

        private void Awake()
        {
            InitUI();
        }

        private void Start()
        {
            infos = PlayerService.player.friendInfos;

            LoopScroll.prefabSource = LoopScrollPrefabSource;
            LoopScroll.dataSource = this;
            LoopScroll.totalCount = infos.Count;
            LoopScroll.RefillCells();
        }

        public void ProvideData(Transform tra, int idx)
        {
            var bar = tra.GetComponent<UIUserInfoBar>();
            bar.Set(infos[idx]);
        }
    }
}