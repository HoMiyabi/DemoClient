public class MsgMetaItem
{
    public readonly uint cmdId;
    public readonly Google.Protobuf.MessageParser parser;
    public readonly bool isRsp;

    public MsgMetaItem(uint cmdId, Google.Protobuf.MessageParser parser, bool isRsp)
    {
        this.cmdId = cmdId;
        this.parser = parser;
        this.isRsp = isRsp;
    }
}