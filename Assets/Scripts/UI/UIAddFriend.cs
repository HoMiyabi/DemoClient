using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.UI.Panel;
using UnityEngine;

namespace Kirara.UI
{
    public class UIAddFriend : BasePanel
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI                 FriendRequestCountText;
        private TMPro.TMP_InputField                  SearchInput;
        private UnityEngine.UI.Button                 SearchBtn;
        private UnityEngine.UI.LoopVerticalScrollRect LoopVerticalScrollRect;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                  = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            FriendRequestCountText = c.Q<TMPro.TextMeshProUGUI>(0, "FriendRequestCountText");
            SearchInput            = c.Q<TMPro.TMP_InputField>(1, "SearchInput");
            SearchBtn              = c.Q<UnityEngine.UI.Button>(2, "SearchBtn");
            LoopVerticalScrollRect = c.Q<UnityEngine.UI.LoopVerticalScrollRect>(3, "LoopVerticalScrollRect");
        }
        #endregion

        public GameObject UserItemPrefab;

        private List<SocialPlayer> friendRequests;

        private void Awake()
        {
            BindUI();

            friendRequests = PlayerService.Player.FriendRequests;

            SearchBtn.onClick.AddListener(UniTask.UnityAction(SearchBtn_onClick));

            LoopVerticalScrollRect.prefabSource = new LoopScrollPool(UserItemPrefab, transform);
            LoopVerticalScrollRect.dataSource = new LoopScrollDataSourceHandler(ProvideData);

            UpdateUI();
            PlayerService.Player.OnFriendRequestsChanged += UpdateUI;
        }

        private void ProvideData(Transform trans, int idx)
        {
            var item = trans.GetComponent<SP_UserFriendReqBar>();
            item.Set(friendRequests[idx]);
        }

        private void UpdateUI()
        {
            FriendRequestCountText.text = $"好友请求数量 {friendRequests.Count}";

            LoopVerticalScrollRect.totalCount = friendRequests.Count;
            LoopVerticalScrollRect.RefillCells();
        }

        private async UniTaskVoid SearchBtn_onClick()
        {
            string text = SearchInput.text;
            var rsp = await NetFn.ReqSearchPlayer(new ReqSearchPlayer
            {
                Username = text
            });
            Debug.Log(rsp.SocialPlayer);

            var rsp1 = await NetFn.ReqSendAddFriend(new ReqSendAddFriend
            {
                TargetUid = rsp.SocialPlayer.Uid
            });
            Debug.Log(rsp1);
        }
    }
}