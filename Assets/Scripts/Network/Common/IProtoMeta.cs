using System;

public interface IProtoMeta
{
    uint GetMsgId(Type msgType);
    Google.Protobuf.MessageParser GetParser(uint msgId);
    bool IsRsp(uint msgId);
}