// 本文件为生成的代码，所有修改都会丢失
using Cysharp.Threading.Tasks;
using Kirara.Manager;
public static partial class NetFn
{
    public static UniTask<Pong> Ping(Ping req)
    {
        return CallAsync<Pong>(MsgCmdId.Ping, req);
    }
    public static UniTask<RspRegister> ReqRegister(ReqRegister req)
    {
        return CallAsync<RspRegister>(MsgCmdId.ReqRegister, req);
    }
    public static UniTask<RspLogin> ReqLogin(ReqLogin req)
    {
        return CallAsync<RspLogin>(MsgCmdId.ReqLogin, req);
    }
    public static UniTask<RspGetPlayerData> ReqGetPlayerData(ReqGetPlayerData req)
    {
        return CallAsync<RspGetPlayerData>(MsgCmdId.ReqGetPlayerData, req);
    }
    public static UniTask<RspGetExchangeItems> ReqGetExchangeItems(ReqGetExchangeItems req)
    {
        return CallAsync<RspGetExchangeItems>(MsgCmdId.ReqGetExchangeItems, req);
    }
    public static UniTask<RspExchange> ReqExchange(ReqExchange req)
    {
        return CallAsync<RspExchange>(MsgCmdId.ReqExchange, req);
    }
    public static UniTask<RspSearchPlayer> ReqSearchPlayer(ReqSearchPlayer req)
    {
        return CallAsync<RspSearchPlayer>(MsgCmdId.ReqSearchPlayer, req);
    }
    public static UniTask<RspSendAddFriend> ReqSendAddFriend(ReqSendAddFriend req)
    {
        return CallAsync<RspSendAddFriend>(MsgCmdId.ReqSendAddFriend, req);
    }
    public static UniTask<RspAcceptAddFriend> ReqAcceptAddFriend(ReqAcceptAddFriend req)
    {
        return CallAsync<RspAcceptAddFriend>(MsgCmdId.ReqAcceptAddFriend, req);
    }
    public static UniTask<RspRefuseAddFriend> ReqRefuseAddFriend(ReqRefuseAddFriend req)
    {
        return CallAsync<RspRefuseAddFriend>(MsgCmdId.ReqRefuseAddFriend, req);
    }
    public static UniTask<RspRemoveFriend> ReqRemoveFriend(ReqRemoveFriend req)
    {
        return CallAsync<RspRemoveFriend>(MsgCmdId.ReqRemoveFriend, req);
    }
    public static UniTask<RspModifySignature> ReqModifySignature(ReqModifySignature req)
    {
        return CallAsync<RspModifySignature>(MsgCmdId.ReqModifySignature, req);
    }
    public static UniTask<RspModifyPassword> ReqModifyPassword(ReqModifyPassword req)
    {
        return CallAsync<RspModifyPassword>(MsgCmdId.ReqModifyPassword, req);
    }
    public static UniTask<RspModifyAvatar> ReqModifyAvatar(ReqModifyAvatar req)
    {
        return CallAsync<RspModifyAvatar>(MsgCmdId.ReqModifyAvatar, req);
    }
    public static UniTask<RspSendChatMsg> ReqSendChatMsg(ReqSendChatMsg req)
    {
        return CallAsync<RspSendChatMsg>(MsgCmdId.ReqSendChatMsg, req);
    }
    public static UniTask<RspRoleRemoveDisc> ReqRoleRemoveDisc(ReqRoleRemoveDisc req)
    {
        return CallAsync<RspRoleRemoveDisc>(MsgCmdId.ReqRoleRemoveDisc, req);
    }
    public static UniTask<RspRoleEquipDisc> ReqRoleEquipDisc(ReqRoleEquipDisc req)
    {
        return CallAsync<RspRoleEquipDisc>(MsgCmdId.ReqRoleEquipDisc, req);
    }
    public static UniTask<RspRoleRemoveWeapon> ReqRoleRemoveWeapon(ReqRoleRemoveWeapon req)
    {
        return CallAsync<RspRoleRemoveWeapon>(MsgCmdId.ReqRoleRemoveWeapon, req);
    }
    public static UniTask<RspRoleEquipWeapon> ReqRoleEquipWeapon(ReqRoleEquipWeapon req)
    {
        return CallAsync<RspRoleEquipWeapon>(MsgCmdId.ReqRoleEquipWeapon, req);
    }
    public static UniTask<RspStartQuest> ReqStartQuest(ReqStartQuest req)
    {
        return CallAsync<RspStartQuest>(MsgCmdId.ReqStartQuest, req);
    }
    public static UniTask<RspUpgradeDisc> ReqUpgradeDisc(ReqUpgradeDisc req)
    {
        return CallAsync<RspUpgradeDisc>(MsgCmdId.ReqUpgradeDisc, req);
    }
}