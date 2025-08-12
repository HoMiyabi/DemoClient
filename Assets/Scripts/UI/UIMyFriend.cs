using System.Collections.Generic;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI
{
    public class UIMyFriend : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI             FriendCountText;
        private KiraraLoopScroll.LinearScrollView ScrollView;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            FriendCountText = c.Q<TMPro.TextMeshProUGUI>(0, "FriendCountText");
            ScrollView      = c.Q<KiraraLoopScroll.LinearScrollView>(1, "ScrollView");
        }
        #endregion

        public GameObject UserInfoBarItemPrefab;

        private List<SocialPlayer> friends;

        private void Awake()
        {
            BindUI();

            friends = PlayerService.Player.Friends;
            Debug.Log("friends.Count = " + friends.Count);

            ScrollView.SetGOSource(new LoopScrollGOPool(UserInfoBarItemPrefab, transform));
            ScrollView.provideData = ProvideData;
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

            ScrollView._totalCount = friends.Count;
        }

        private void ProvideData(GameObject go, int idx)
        {
            var bar = go.GetComponent<UIUserInfoItem>();
            bar.Set(friends[idx]);
        }
    }
}