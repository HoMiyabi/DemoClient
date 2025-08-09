using System.Collections.Generic;
using Kirara.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIMyFriend : MonoBehaviour, LoopScrollDataSource
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI                 FriendCountText;
        private UnityEngine.UI.LoopVerticalScrollRect LoopScroll;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            FriendCountText = c.Q<TMPro.TextMeshProUGUI>(0, "FriendCountText");
            LoopScroll      = c.Q<UnityEngine.UI.LoopVerticalScrollRect>(1, "LoopScroll");
        }
        #endregion

        public GameObject UserInfoBarItemPrefab;

        private List<SocialPlayer> friends;

        private void Awake()
        {
            BindUI();
        }

        private void Start()
        {
            friends = PlayerService.Player.Friends;
            Debug.Log("friends.Count = " + friends.Count);

            LoopScroll.prefabSource = new LoopScrollPool(UserInfoBarItemPrefab, transform);
            LoopScroll.dataSource = this;
            UpdateUI();
            PlayerService.Player.OnFriendsChanged += UpdateUI;
        }

        private void OnDestroy()
        {
            PlayerService.Player.OnFriendsChanged -= UpdateUI;
        }

        private void UpdateUI()
        {
            FriendCountText.text = $"好友数量 {friends.Count}";

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