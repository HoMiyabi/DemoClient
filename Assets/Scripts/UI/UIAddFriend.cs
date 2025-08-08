using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIAddFriend : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI        FriendRequestCountText;
        private TMP_InputField         SearchInput;
        private Button                 SearchBtn;
        private LoopVerticalScrollRect LoopVerticalScrollRect;
        private void InitUI()
        {
            var c                  = GetComponent<KiraraDirectBinder>();
            FriendRequestCountText = c.Q<TextMeshProUGUI>(0, "FriendRequestCountText");
            SearchInput            = c.Q<TMP_InputField>(1, "SearchInput");
            SearchBtn              = c.Q<Button>(2, "SearchBtn");
            LoopVerticalScrollRect = c.Q<LoopVerticalScrollRect>(3, "LoopVerticalScrollRect");
        }
        #endregion

        public GameObject UserItemPrefab;

        private List<SocialPlayer> friendRequests;

        private void Awake()
        {
            InitUI();

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