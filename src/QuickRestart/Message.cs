using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MieMod.QuickRestart.Multiplayer;

public struct QuickRestartMessage :
    INetMessage,
    IPacketSerializable
{
    public ulong playerId;

    public bool ShouldBroadcast => true;

    public NetTransferMode Mode => NetTransferMode.Reliable;

    public LogLevel LogLevel => LogLevel.VeryDebug;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(playerId);
    }

    public void Deserialize(PacketReader reader)
    {
        this.playerId = reader.ReadULong();
    }

    public override string ToString()
    {
        return $"[MIEMOD]: QuickRestartMessage: playerId={playerId}";
    }
}