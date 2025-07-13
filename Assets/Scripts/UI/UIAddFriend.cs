using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIAddFriend : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI FriendRequestCountText;
        private TMP_InputField  SearchInput;
        private Button          SearchBtn;
        private void InitUI()
        {
            var c                  = GetComponent<KiraraDirectBinder>();
            FriendRequestCountText = c.Q<TextMeshProUGUI>(0, "FriendRequestCountText");
            SearchInput            = c.Q<TMP_InputField>(1, "SearchInput");
            SearchBtn              = c.Q<Button>(2, "SearchBtn");
        }
        #endregion

        private void Awake()
        {
            InitUI();

            SearchBtn.onClick.AddListener(UniTask.UnityAction(SearchBtn_onClick));
        }

        private async UniTaskVoid SearchBtn_onClick()
        {
            string text = SearchInput.text;
            var rsp = await NetFn.ReqSearchPlayer(new ReqSearchPlayer
            {
                Username = text
            });
            Debug.Log($"{rsp.SocialPlayers}");
        }
    }
}