// 本文件为生成的代码，所有修改都会丢失
using Cysharp.Threading.Tasks;
using Kirara.Manager;
public static partial class NetFn
{
    public static UniTask<Pong> Ping(Ping req)
    {
        return NetMgr.Instance.session.CallAsync<Pong>(ProtoMsgId.Ping, req);
    }
    public static UniTask<RspRegister> ReqRegister(ReqRegister req)
    {
        return NetMgr.Instance.session.CallAsync<RspRegister>(ProtoMsgId.ReqRegister, req);
    }
    public static UniTask<RspLogin> ReqLogin(ReqLogin req)
    {
        return NetMgr.Instance.session.CallAsync<RspLogin>(ProtoMsgId.ReqLogin, req);
    }
    public static UniTask<RspGetPlayerData> ReqGetPlayerData(ReqGetPlayerData req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetPlayerData>(ProtoMsgId.ReqGetPlayerData, req);
    }
    public static UniTask<RspGetExchangeItems> ReqGetExchangeItems(ReqGetExchangeItems req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetExchangeItems>(ProtoMsgId.ReqGetExchangeItems, req);
    }
    public static UniTask<RspExchange> ReqExchange(ReqExchange req)
    {
        return NetMgr.Instance.session.CallAsync<RspExchange>(ProtoMsgId.ReqExchange, req);
    }
    public static UniTask<RspSearchPlayer> ReqSearchPlayer(ReqSearchPlayer req)
    {
        return NetMgr.Instance.session.CallAsync<RspSearchPlayer>(ProtoMsgId.ReqSearchPlayer, req);
    }
    public static UniTask<RspGetFriendInfos> ReqGetFriendInfos(ReqGetFriendInfos req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetFriendInfos>(ProtoMsgId.ReqGetFriendInfos, req);
    }
    public static UniTask<RspSendAddFriend> ReqSendAddFriend(ReqSendAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspSendAddFriend>(ProtoMsgId.ReqSendAddFriend, req);
    }
    public static UniTask<RspAcceptAddFriend> ReqAcceptAddFriend(ReqAcceptAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspAcceptAddFriend>(ProtoMsgId.ReqAcceptAddFriend, req);
    }
    public static UniTask<RspRefuseAddFriend> ReqRefuseAddFriend(ReqRefuseAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspRefuseAddFriend>(ProtoMsgId.ReqRefuseAddFriend, req);
    }
    public static UniTask<RspDeleteFriend> ReqDeleteFriend(ReqDeleteFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspDeleteFriend>(ProtoMsgId.ReqDeleteFriend, req);
    }
    public static UniTask<RspModifySignature> ReqModifySignature(ReqModifySignature req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifySignature>(ProtoMsgId.ReqModifySignature, req);
    }
    public static UniTask<RspModifyPassword> ReqModifyPassword(ReqModifyPassword req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifyPassword>(ProtoMsgId.ReqModifyPassword, req);
    }
    public static UniTask<RspModifyAvatar> ReqModifyAvatar(ReqModifyAvatar req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifyAvatar>(ProtoMsgId.ReqModifyAvatar, req);
    }
    public static UniTask<RspSendChatMsg> ReqSendChatMsg(ReqSendChatMsg req)
    {
        return NetMgr.Instance.session.CallAsync<RspSendChatMsg>(ProtoMsgId.ReqSendChatMsg, req);
    }
    public static UniTask<RspGetChatRecords> ReqGetChatRecords(ReqGetChatRecords req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetChatRecords>(ProtoMsgId.ReqGetChatRecords, req);
    }
    public static UniTask<RspRoleRemoveDisc> ReqRoleRemoveDisc(ReqRoleRemoveDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleRemoveDisc>(ProtoMsgId.ReqRoleRemoveDisc, req);
    }
    public static UniTask<RspRoleEquipDisc> ReqRoleEquipDisc(ReqRoleEquipDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleEquipDisc>(ProtoMsgId.ReqRoleEquipDisc, req);
    }
    public static UniTask<RspRoleRemoveWeapon> ReqRoleRemoveWeapon(ReqRoleRemoveWeapon req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleRemoveWeapon>(ProtoMsgId.ReqRoleRemoveWeapon, req);
    }
    public static UniTask<RspRoleEquipWeapon> ReqRoleEquipWeapon(ReqRoleEquipWeapon req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleEquipWeapon>(ProtoMsgId.ReqRoleEquipWeapon, req);
    }
    public static UniTask<RspStartQuest> ReqStartQuest(ReqStartQuest req)
    {
        return NetMgr.Instance.session.CallAsync<RspStartQuest>(ProtoMsgId.ReqStartQuest, req);
    }
    public static UniTask<RspUpgradeDisc> ReqUpgradeDisc(ReqUpgradeDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspUpgradeDisc>(ProtoMsgId.ReqUpgradeDisc, req);
    }
}