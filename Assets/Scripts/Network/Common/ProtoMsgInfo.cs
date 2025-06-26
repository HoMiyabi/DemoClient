using System;
using Google.Protobuf;

public class ProtoMsgInfo
{
    public uint msgId;
    public Type msgType;
    public MessageParser msgParser;

    public uint rspId;
    public Type rspType;

    public ProtoMsgInfo(uint msgId, Type msgType, MessageParser msgParser, uint rspId, Type rspType)
    {
        this.msgId = msgId;
        this.msgType = msgType;
        this.msgParser = msgParser;
        this.rspId = rspId;
        this.rspType = rspType;
    }
}