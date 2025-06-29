public class MsgMetaData
{
    public readonly uint cmdId;
    public readonly Google.Protobuf.MessageParser parser;
    public readonly bool isRsp;

    public MsgMetaData(uint cmdId, Google.Protobuf.MessageParser parser, bool isRsp)
    {
        this.cmdId = cmdId;
        this.parser = parser;
        this.isRsp = isRsp;
    }
}