using System.Collections.Generic;
using Kirara.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIMyFriend : MonoBehaviour, LoopScrollDataSource
    {
        #region View
        private TextMeshProUGUI              FriendCountText;
        private LoopVerticalScrollRect       LoopScroll;
        private SimpleLoopScrollPrefabSource LoopScrollPrefabSource;
        private void InitUI()
        {
            var c                  = GetComponent<KiraraDirectBinder>();
            FriendCountText        = c.Q<TextMeshProUGUI>(0, "FriendCountText");
            LoopScroll             = c.Q<LoopVerticalScrollRect>(1, "LoopScroll");
            LoopScrollPrefabSource = c.Q<SimpleLoopScrollPrefabSource>(2, "LoopScrollPrefabSource");
        }
        #endregion

        private List<SocialPlayer> friends;

        private void Awake()
        {
            InitUI();
        }

        private void Start()
        {
            friends = PlayerService.Player.Friends;

            LoopScroll.prefabSource = LoopScrollPrefabSource;
            LoopScroll.dataSource = this;
            LoopScroll.totalCount = friends.Count;
            LoopScroll.RefillCells();
        }

        public void ProvideData(Transform tra, int idx)
        {
            var bar = tra.GetComponent<UIUserInfoBar>();
            bar.Set(friends[idx]);
        }
    }
}