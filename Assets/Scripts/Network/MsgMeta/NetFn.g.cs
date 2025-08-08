// 本文件为生成的代码，所有修改都会丢失
using Cysharp.Threading.Tasks;
using Kirara.Manager;
public static partial class NetFn
{
    public static UniTask<Pong> Ping(Ping req)
    {
        return NetMgr.Instance.session.CallAsync<Pong>(MsgCmdId.Ping, req);
    }
    public static UniTask<RspRegister> ReqRegister(ReqRegister req)
    {
        return NetMgr.Instance.session.CallAsync<RspRegister>(MsgCmdId.ReqRegister, req);
    }
    public static UniTask<RspLogin> ReqLogin(ReqLogin req)
    {
        return NetMgr.Instance.session.CallAsync<RspLogin>(MsgCmdId.ReqLogin, req);
    }
    public static UniTask<RspGetPlayerData> ReqGetPlayerData(ReqGetPlayerData req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetPlayerData>(MsgCmdId.ReqGetPlayerData, req);
    }
    public static UniTask<RspGetExchangeItems> ReqGetExchangeItems(ReqGetExchangeItems req)
    {
        return NetMgr.Instance.session.CallAsync<RspGetExchangeItems>(MsgCmdId.ReqGetExchangeItems, req);
    }
    public static UniTask<RspExchange> ReqExchange(ReqExchange req)
    {
        return NetMgr.Instance.session.CallAsync<RspExchange>(MsgCmdId.ReqExchange, req);
    }
    public static UniTask<RspSearchPlayer> ReqSearchPlayer(ReqSearchPlayer req)
    {
        return NetMgr.Instance.session.CallAsync<RspSearchPlayer>(MsgCmdId.ReqSearchPlayer, req);
    }
    public static UniTask<RspSendAddFriend> ReqSendAddFriend(ReqSendAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspSendAddFriend>(MsgCmdId.ReqSendAddFriend, req);
    }
    public static UniTask<RspAcceptAddFriend> ReqAcceptAddFriend(ReqAcceptAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspAcceptAddFriend>(MsgCmdId.ReqAcceptAddFriend, req);
    }
    public static UniTask<RspRefuseAddFriend> ReqRefuseAddFriend(ReqRefuseAddFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspRefuseAddFriend>(MsgCmdId.ReqRefuseAddFriend, req);
    }
    public static UniTask<RspRemoveFriend> ReqRemoveFriend(ReqRemoveFriend req)
    {
        return NetMgr.Instance.session.CallAsync<RspRemoveFriend>(MsgCmdId.ReqRemoveFriend, req);
    }
    public static UniTask<RspModifySignature> ReqModifySignature(ReqModifySignature req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifySignature>(MsgCmdId.ReqModifySignature, req);
    }
    public static UniTask<RspModifyPassword> ReqModifyPassword(ReqModifyPassword req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifyPassword>(MsgCmdId.ReqModifyPassword, req);
    }
    public static UniTask<RspModifyAvatar> ReqModifyAvatar(ReqModifyAvatar req)
    {
        return NetMgr.Instance.session.CallAsync<RspModifyAvatar>(MsgCmdId.ReqModifyAvatar, req);
    }
    public static UniTask<RspSendChatMsg> ReqSendChatMsg(ReqSendChatMsg req)
    {
        return NetMgr.Instance.session.CallAsync<RspSendChatMsg>(MsgCmdId.ReqSendChatMsg, req);
    }
    public static UniTask<RspRoleRemoveDisc> ReqRoleRemoveDisc(ReqRoleRemoveDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleRemoveDisc>(MsgCmdId.ReqRoleRemoveDisc, req);
    }
    public static UniTask<RspRoleEquipDisc> ReqRoleEquipDisc(ReqRoleEquipDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleEquipDisc>(MsgCmdId.ReqRoleEquipDisc, req);
    }
    public static UniTask<RspRoleRemoveWeapon> ReqRoleRemoveWeapon(ReqRoleRemoveWeapon req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleRemoveWeapon>(MsgCmdId.ReqRoleRemoveWeapon, req);
    }
    public static UniTask<RspRoleEquipWeapon> ReqRoleEquipWeapon(ReqRoleEquipWeapon req)
    {
        return NetMgr.Instance.session.CallAsync<RspRoleEquipWeapon>(MsgCmdId.ReqRoleEquipWeapon, req);
    }
    public static UniTask<RspStartQuest> ReqStartQuest(ReqStartQuest req)
    {
        return NetMgr.Instance.session.CallAsync<RspStartQuest>(MsgCmdId.ReqStartQuest, req);
    }
    public static UniTask<RspUpgradeDisc> ReqUpgradeDisc(ReqUpgradeDisc req)
    {
        return NetMgr.Instance.session.CallAsync<RspUpgradeDisc>(MsgCmdId.ReqUpgradeDisc, req);
    }
}