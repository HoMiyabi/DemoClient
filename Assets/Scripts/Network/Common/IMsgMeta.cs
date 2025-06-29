using System;

public interface IMsgMeta
{
    bool TryGetCmdId(Type type, out uint cmdId);
    Google.Protobuf.MessageParser GetParser(uint cmdId);
    bool IsRsp(uint cmdId);
}