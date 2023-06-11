using Google.Protobuf;

public interface IPacketHandler
{
    public void Process(IMessage packet);
}