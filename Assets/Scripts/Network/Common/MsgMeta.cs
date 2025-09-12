using System;
using System.Collections.Generic;

public class MsgMeta
{
    public MsgMetaItem[] items;
    public Dictionary<uint, MsgMetaItem> cmdIdToItem;
    public Dictionary<Type, MsgMetaItem> typeToItem;

    public bool TryGetCmdId(Type type, out uint cmdId)
    {
        if (typeToItem.TryGetValue(type, out var metaData))
        {
            cmdId = metaData.cmdId;
            return true;
        }
        cmdId = 0;
        return false;
    }

    public Google.Protobuf.MessageParser GetParser(uint cmdId)
    {
        if (cmdIdToItem.TryGetValue(cmdId, out var metaItem))
        {
            return metaItem.parser;
        }
        return null;
    }

    public bool IsRsp(uint cmdId)
    {
        if (cmdIdToItem.TryGetValue(cmdId, out var metaItem))
        {
            return metaItem.isRsp;
        }
        return false;
    }
}