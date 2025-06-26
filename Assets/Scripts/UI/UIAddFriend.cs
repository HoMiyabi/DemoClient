using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIAddFriend : MonoBehaviour
    {
        private TextMeshProUGUI FriendRequestCountText;
        private TMP_InputField SearchInput;
        private Button SearchBtn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            FriendRequestCountText = c.Q<TextMeshProUGUI>("FriendRequestCountText");
            SearchInput = c.Q<TMP_InputField>("SearchInput");
            SearchBtn = c.Q<Button>("SearchBtn");
        }

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
            Debug.Log($"{rsp.OtherPlayerInfos}");
        }
    }
}