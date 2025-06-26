// 本文件为生成的代码，所有修改都会丢失
using Cysharp.Threading.Tasks;
using Kirara.Manager;
public static partial class NetFn
{
    public static async UniTask<Pong> Ping(Ping req)
    {
        return (Pong)await NetMgr.Instance.session.CallAsync(ProtoMsgId.Ping, req);
    }
    public static async UniTask<RspRegister> ReqRegister(ReqRegister req)
    {
        return (RspRegister)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRegister, req);
    }
    public static async UniTask<RspLogin> ReqLogin(ReqLogin req)
    {
        return (RspLogin)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqLogin, req);
    }
    public static async UniTask<RspGetPlayerData> ReqGetPlayerData(ReqGetPlayerData req)
    {
        return (RspGetPlayerData)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqGetPlayerData, req);
    }
    public static async UniTask<RspGetExchangeItems> ReqGetExchangeItems(ReqGetExchangeItems req)
    {
        return (RspGetExchangeItems)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqGetExchangeItems, req);
    }
    public static async UniTask<RspExchange> ReqExchange(ReqExchange req)
    {
        return (RspExchange)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqExchange, req);
    }
    public static async UniTask<RspSearchPlayer> ReqSearchPlayer(ReqSearchPlayer req)
    {
        return (RspSearchPlayer)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqSearchPlayer, req);
    }
    public static async UniTask<RspGetFriendInfos> ReqGetFriendInfos(ReqGetFriendInfos req)
    {
        return (RspGetFriendInfos)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqGetFriendInfos, req);
    }
    public static async UniTask<RspSendAddFriend> ReqSendAddFriend(ReqSendAddFriend req)
    {
        return (RspSendAddFriend)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqSendAddFriend, req);
    }
    public static async UniTask<RspAcceptAddFriend> ReqAcceptAddFriend(ReqAcceptAddFriend req)
    {
        return (RspAcceptAddFriend)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqAcceptAddFriend, req);
    }
    public static async UniTask<RspRefuseAddFriend> ReqRefuseAddFriend(ReqRefuseAddFriend req)
    {
        return (RspRefuseAddFriend)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRefuseAddFriend, req);
    }
    public static async UniTask<RspDeleteFriend> ReqDeleteFriend(ReqDeleteFriend req)
    {
        return (RspDeleteFriend)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqDeleteFriend, req);
    }
    public static async UniTask<RspModifySignature> ReqModifySignature(ReqModifySignature req)
    {
        return (RspModifySignature)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqModifySignature, req);
    }
    public static async UniTask<RspModifyPassword> ReqModifyPassword(ReqModifyPassword req)
    {
        return (RspModifyPassword)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqModifyPassword, req);
    }
    public static async UniTask<RspModifyAvatar> ReqModifyAvatar(ReqModifyAvatar req)
    {
        return (RspModifyAvatar)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqModifyAvatar, req);
    }
    public static async UniTask<RspSendChatMsg> ReqSendChatMsg(ReqSendChatMsg req)
    {
        return (RspSendChatMsg)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqSendChatMsg, req);
    }
    public static async UniTask<RspGetChatRecords> ReqGetChatRecords(ReqGetChatRecords req)
    {
        return (RspGetChatRecords)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqGetChatRecords, req);
    }
    public static async UniTask<RspRoleRemoveDisc> ReqRoleRemoveDisc(ReqRoleRemoveDisc req)
    {
        return (RspRoleRemoveDisc)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRoleRemoveDisc, req);
    }
    public static async UniTask<RspRoleEquipDisc> ReqRoleEquipDisc(ReqRoleEquipDisc req)
    {
        return (RspRoleEquipDisc)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRoleEquipDisc, req);
    }
    public static async UniTask<RspRoleRemoveWeapon> ReqRoleRemoveWeapon(ReqRoleRemoveWeapon req)
    {
        return (RspRoleRemoveWeapon)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRoleRemoveWeapon, req);
    }
    public static async UniTask<RspRoleEquipWeapon> ReqRoleEquipWeapon(ReqRoleEquipWeapon req)
    {
        return (RspRoleEquipWeapon)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqRoleEquipWeapon, req);
    }
    public static async UniTask<RspStartQuest> ReqStartQuest(ReqStartQuest req)
    {
        return (RspStartQuest)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqStartQuest, req);
    }
    public static async UniTask<RspUpgradeDisc> ReqUpgradeDisc(ReqUpgradeDisc req)
    {
        return (RspUpgradeDisc)await NetMgr.Instance.session.CallAsync(ProtoMsgId.ReqUpgradeDisc, req);
    }
}
