using Cysharp.Threading.Tasks;
using Kirara.Model;
using Manager;
using UnityEngine;

public class SP_UserFriendReqBar : MonoBehaviour
{
    #region View
    private bool _isBound;
    private UnityEngine.UI.Button AvatarBtn;
    private UnityEngine.UI.Image  AvatarImg;
    private TMPro.TextMeshProUGUI SignatureText;
    private TMPro.TextMeshProUGUI UsernameText;
    private TMPro.TextMeshProUGUI UIUserOnlineStatus;
    private UnityEngine.UI.Button RefuseBtn;
    private UnityEngine.UI.Button AcceptBtn;
    public void BindUI()
    {
        if (_isBound) return;
        _isBound = true;
        var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
        AvatarBtn          = c.Q<UnityEngine.UI.Button>(0, "AvatarBtn");
        AvatarImg          = c.Q<UnityEngine.UI.Image>(1, "AvatarImg");
        SignatureText      = c.Q<TMPro.TextMeshProUGUI>(2, "SignatureText");
        UsernameText       = c.Q<TMPro.TextMeshProUGUI>(3, "UsernameText");
        UIUserOnlineStatus = c.Q<TMPro.TextMeshProUGUI>(4, "UIUserOnlineStatus");
        RefuseBtn          = c.Q<UnityEngine.UI.Button>(5, "RefuseBtn");
        AcceptBtn          = c.Q<UnityEngine.UI.Button>(6, "AcceptBtn");
    }
    #endregion

    private SocialPlayer player;

    public void Set(SocialPlayer player)
    {
        BindUI();

        this.player = player;
        var handle = ConfigAsset.GetIconInterKnotRole(player.AvatarCid);
        AvatarImg.sprite = handle.AssetObject as Sprite;

        SignatureText.text = player.Signature;
        UsernameText.text = player.Username;
        UIUserOnlineStatus.text = player.IsOnline ? "在线" : "离线";

        RefuseBtn.onClick.AddListener(() => RefuseBtn_onClick().Forget());
        AcceptBtn.onClick.AddListener(() => AcceptBtn_onClick().Forget());
    }

    private async UniTaskVoid RefuseBtn_onClick()
    {
        await NetFn.ReqRefuseAddFriend(new ReqRefuseAddFriend
        {
            SenderUid = player.Uid
        });
    }

    private async UniTaskVoid AcceptBtn_onClick()
    {
        await NetFn.ReqAcceptAddFriend(new ReqAcceptAddFriend
        {
            SenderUid = player.Uid
        });
    }
}
